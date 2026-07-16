using System.Data;
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

        public Task<List<Contacto>> ObtenerContactoAsync(string filtroBusqueda)
        {
            try
            {
                DataTable dtContacto = _servicioAccesoDatos.TraerTablaParametros("Contactos", "IdContacto, NombreContacto, CelularContacto", $"CelularContacto like '%{filtroBusqueda}%'");

                List<Contacto> contactos = new();

                if (dtContacto == null || dtContacto.Rows.Count == 0)
                    return Task.FromResult(contactos);

                foreach (DataRow row in dtContacto.Rows)
                {
                    contactos.Add(new Contacto
                    {
                        IdContacto = row.Field<int>("IdContacto"),
                        NombreContacto = row.Field<string>("NombreContacto") ?? string.Empty,
                        CelularContacto = row.Field<string>("CelularContacto") ?? string.Empty
                        // Agrega aquí las demás propiedades
                    });
                }

                return Task.FromResult(contactos);
            }
            catch (Exception ex)
            {
                // Si tienes un logger, regístralo aquí.
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
