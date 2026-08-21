using System.Data;
using WhatsappComercial.Enums;
using WhatsappComercial.Interfaces.Contactos;
using WhatsappComercial.Modelos;
using WhatsappComercial.Servicios.AccesoADatos;

namespace WhatsappComercial.Servicios.Contactos
{
    public class BusquedaContactos : IBusquedaContactos
    {
        private readonly ServicioAccesoDatos _servicioAccesoDatos;

        public BusquedaContactos(ServicioAccesoDatos servicioAccesoDatos)
        {
            _servicioAccesoDatos = servicioAccesoDatos;
        }

        public Task<List<Contacto>> ObtenerBeneficiarioAsync(string filtroBusqueda)
        {
            DataTable dtContacto = _servicioAccesoDatos.TraerTablaParametros("Clientes", "IdCliente, NombreCompletoCliente, NumCelularCliente", $"numCelularcliente like '%{filtroBusqueda}%' or NumIdentificacionCliente like '%{filtroBusqueda}%'");
            DataTable dtBeneficiario = new();
            if (dtContacto.Rows.Count > 0)
            {
                int idClienteTitular = Convert.ToInt32(dtContacto.Rows[0]["IdCliente"]);
                dtBeneficiario = _servicioAccesoDatos.TraerTablaParametros("Beneficiarios", "IdBeneficiario, NombreCompleto, Telefono, Identificacion", $"IdCliente = {idClienteTitular}");

            }

            List<Contacto> contactos = new();

            if (dtBeneficiario == null || dtBeneficiario.Rows.Count == 0)
                return Task.FromResult(contactos);

            foreach (DataRow row in dtBeneficiario.Rows)
            {
                contactos.Add(new Contacto
                {
                    IdContacto = row.Field<int>("IdBeneficiario"),
                    NombreContacto = row.Field<string>("NombreCompleto") ?? string.Empty,
                    CelularContacto = row.Field<string>("Telefono") ?? string.Empty,
                    NroIdentificacion = row.Field<string>("Identificacion") ?? string.Empty
                });
            }

            return Task.FromResult(contactos);
        }

        public Task<List<Contacto>> ObtenerClienteCobranzasAsync(string filtroBusqueda)
        {
            DataTable dtClienteTitular = _servicioAccesoDatos.TraerTablaParametros("BaseCobranzas", "IdRegistro, Nombre, Telefonos", $"Telefonos like '%{filtroBusqueda}%' or Cedula like '%{filtroBusqueda}%'");

            List<Contacto> contactos = new();

            if (dtClienteTitular == null || dtClienteTitular.Rows.Count == 0)
                return Task.FromResult(contactos);

            foreach (DataRow row in dtClienteTitular.Rows)
            {
                contactos.Add(new Contacto
                {
                    IdContacto = row.Field<int>("IdRegistro"),
                    NombreContacto = row.Field<string>("Nombre") ?? string.Empty,
                    CelularContacto = row.Field<string>("Telefonos") ?? string.Empty,
                    NroIdentificacion = row.Field<string>("cedula") ?? string.Empty
                });
            }

            return Task.FromResult(contactos);
        }

        public Task<List<Contacto>> ObtenerClientePagaduriasAsync(string filtroBusqueda)
        {
            DataTable dtClienteTitular = _servicioAccesoDatos.TraerTablaParametros("BaseTelemercadeo", "IdBaseTelemercadeo, Nombre, Celular, Identificacion", $"Celular like '%{filtroBusqueda}%' or Identificacion like '%{filtroBusqueda}%'");

            List<Contacto> contactos = new();

            if (dtClienteTitular == null || dtClienteTitular.Rows.Count == 0)
                return Task.FromResult(contactos);

            foreach (DataRow row in dtClienteTitular.Rows)
            {
                contactos.Add(new Contacto
                {
                    IdContacto = row.Field<int>("IdBaseTelemercadeo"),
                    NombreContacto = row.Field<string>("Nombre") ?? string.Empty,
                    CelularContacto = row.Field<string>("Celular") ?? string.Empty,
                    NroIdentificacion = row.Field<string>("Identificacion") ?? string.Empty
                });
            }

            return Task.FromResult(contactos);
        }

        public Task<List<Contacto>> ObtenerClienteTitularAsync(string filtroBusqueda)
        {

            DataTable dtClienteTitular = _servicioAccesoDatos.TraerTablaParametros("Clientes", "IdCliente, NombreCompletoCliente, NumCelularCliente, NumIdentificacionCliente, Profesion, correoElectronico", $"numCelularcliente like '%{filtroBusqueda}%' or NumIdentificacionCliente like '%{filtroBusqueda}%'");

            List<Contacto> contactos = new();

            if (dtClienteTitular == null || dtClienteTitular.Rows.Count == 0)
                return Task.FromResult(contactos);

            foreach (DataRow row in dtClienteTitular.Rows)
            {
                contactos.Add(new Contacto
                {
                    IdContacto = row.Field<int>("IdCliente"),
                    NombreContacto = row.Field<string>("NombreCompletoCliente") ?? string.Empty,
                    CelularContacto = row.Field<string>("numCelularCliente") ?? string.Empty,
                    NroIdentificacion = row.Field<string>("NumIdentificacionCliente") ?? string.Empty,
                    CorreoElectronico = row.Field<string>("CorreoElectronico") ?? string.Empty
                    
                    // Agrega aquí las demás propiedades
                });
            }

            return Task.FromResult(contactos);
        }

        public Task<List<Contacto>> ObtenerContactoAsync(string filtroBusqueda)
        {
            try
            {
                // Sanitizar comillas simples para prevenir inyección SQL básica en la cláusula Where
                string filtroSanitizado = (filtroBusqueda ?? string.Empty).Replace("'", "''");

                DataTable dtContacto = _servicioAccesoDatos.TraerTablaParametros("Contactos", "IdContacto, Prefijo, NombreContacto, CelularContacto, NroIdentificacion, Genero, NombreInstitucion",
                    $"CelularContacto like '%{filtroSanitizado}%' or NroIdentificacion like '%{filtroBusqueda}%' or NombreContacto like '%{filtroBusqueda}%'");

                List<Contacto> contactos = new();

                if (dtContacto == null || dtContacto.Rows.Count == 0)
                    return Task.FromResult(contactos);

                foreach (DataRow row in dtContacto.Rows)
                {
                    // Manejo seguro del valor entero del género
                    int valorGenero = row.IsNull("Genero") ? 0 : Convert.ToInt32(row["Genero"]);

                    contactos.Add(new Contacto
                    {
                        IdContacto = row.Field<int>("IdContacto"),
                        Prefijo = row.Field<string>("Prefijo") ?? string.Empty,
                        NombreContacto = row.Field<string>("NombreContacto") ?? string.Empty,
                        CelularContacto = row.Field<string>("CelularContacto") ?? string.Empty,
                        NroIdentificacion = row.Field<string>("NroIdentificacion") ?? string.Empty,
                        //CorreoElectronico = row.Field<string>("CorreoElectronico") ?? string.Empty,
                        NombreInstitucion = row.Field<string>("NombreInstitucion") ?? string.Empty,
                        Genero = (GeneroEnum)valorGenero
                    });
                }

                return Task.FromResult(contactos);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
