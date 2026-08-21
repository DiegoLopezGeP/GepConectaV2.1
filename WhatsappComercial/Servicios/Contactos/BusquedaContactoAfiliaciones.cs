using System.Data;
using WhatsappComercial.Interfaces.Contactos;
using WhatsappComercial.Modelos;
using WhatsappComercial.Servicios.AccesoADatos;

namespace WhatsappComercial.Servicios.Contactos
{
    public class BusquedaContactoAfiliaciones : IBusquedaContactoAfiliaciones
    {
        private readonly ServicioAccesoDatos _servicioAccesoDatos;

        public BusquedaContactoAfiliaciones(ServicioAccesoDatos servicioAccesoDatos)
        {
            _servicioAccesoDatos = servicioAccesoDatos;
        }
        public Task<List<Contacto>> BuscarBeneficiarioAfiliaciones(string filtro)
        {
            DataTable dtContacto = _servicioAccesoDatos.TraerTablaParametrosAfiliaciones("Clientes", "IdCliente", $"Identificacion like '%{filtro}%'");

            DataTable dtBeneficiario = new();
            if (dtContacto.Rows.Count > 0)
            {
                // Convert.ToInt32 maneja la conversión desde Decimal, Int64, etc., sin lanzar la excepción
                int idClienteTitular = Convert.ToInt32(dtContacto.Rows[0]["IdCliente"]);
                dtBeneficiario = _servicioAccesoDatos.TraerTablaParametrosAfiliaciones(
                    "Beneficiarios",
                    "IdBeneficiario, NombreBeneficiario, PrimerApellido, SegundoApellido, Telefonos",
                    $"IdCliente = {idClienteTitular}"
                );
            }

            List<Contacto> contactos = new();

            if (dtBeneficiario == null || dtBeneficiario.Rows.Count == 0)
                return Task.FromResult(contactos);

            foreach (DataRow row in dtBeneficiario.Rows)
            {
                // 1. Obtener valores individuales evitando nulos
                string nombre = row.Field<string>("NombreBeneficiario")?.Trim() ?? string.Empty;
                string primerApellido = row.Field<string>("PrimerApellido")?.Trim() ?? string.Empty;
                string segundoApellido = row.Field<string>("SegundoApellido")?.Trim() ?? string.Empty;

                // 2. Concatenar los nombres/apellidos
                string nombreCompleto = string.Join(" ",
                    new[] { nombre, primerApellido, segundoApellido }
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                );

                contactos.Add(new Contacto
                {
                    // Usamos Convert.ToInt32 en lugar de row.Field<int> para tolerar tipos Decimal
                    IdContacto = Convert.ToInt32(row["IdBeneficiario"]),
                    NombreContacto = nombreCompleto,
                    CelularContacto = row.Table.Columns.Contains("Telefonos") ? (row.Field<string>("Telefonos") ?? string.Empty) : string.Empty,
                    NroIdentificacion = row.Table.Columns.Contains("Identificacion") ? (row.Field<string>("Identificacion") ?? string.Empty) : string.Empty,
                });
            }

            return Task.FromResult(contactos);
        }

        public Task<List<Contacto>> BuscarTitularAfiliaciones(string filtro)
        {
            DataTable dtClienteTitular = _servicioAccesoDatos.TraerTablaParametrosAfiliaciones(
                "Clientes",
                "IdCliente, PrimerNombre, SegundNombre, PrimerApellido, SegundoApellido, Celular, Identificacion, Email",
                $"Identificacion like '%{filtro}%'"
            );

            List<Contacto> contactos = new();

            if (dtClienteTitular == null || dtClienteTitular.Rows.Count == 0)
                return Task.FromResult(contactos);

            foreach (DataRow row in dtClienteTitular.Rows)
            {
                // 1. Lectura segura de nombres y apellidos manejando nulos de la BD
                string pNombre = row["PrimerNombre"] != DBNull.Value ? row.Field<string>("PrimerNombre")?.Trim() ?? "" : "";
                string sNombre = row["SegundNombre"] != DBNull.Value ? row.Field<string>("SegundNombre")?.Trim() ?? "" : "";
                string pApellido = row["PrimerApellido"] != DBNull.Value ? row.Field<string>("PrimerApellido")?.Trim() ?? "" : "";
                string sApellido = row["SegundoApellido"] != DBNull.Value ? row.Field<string>("SegundoApellido")?.Trim() ?? "" : "";

                // 2. Concatenación limpia sin espacios dobles o sobrantes
                string nombreCompleto = string.Join(" ", new[] { pNombre, sNombre, pApellido, sApellido }
                    .Where(s => !string.IsNullOrWhiteSpace(s)));

                // 3. Mapeo con los nombres exactos definidos en el SELECT
                contactos.Add(new Contacto
                {
                    IdContacto = row["IdCliente"] != DBNull.Value ? Convert.ToInt32(row["IdCliente"]) : 0,
                    NombreContacto = nombreCompleto,
                    CelularContacto = row["Celular"] != DBNull.Value ? row.Field<string>("Celular") ?? string.Empty : string.Empty,
                    NroIdentificacion = row["Identificacion"] != DBNull.Value ? row.Field<string>("Identificacion") ?? string.Empty : string.Empty,
                    CorreoElectronico = row["Email"] != DBNull.Value ? row.Field<string>("Email") ?? string.Empty : string.Empty
                });
            }

            return Task.FromResult(contactos);
        }
    }
}
