using System.Data;
using WhatsappComercial.Interfaces.Plantillas;
using WhatsappComercial.Servicios.AccesoADatos;

namespace WhatsappComercial.Servicios.Plantillas
{
    public class ObtenerLinkMultimediaPlantilla : IObtenerLinkMultimediaPlantilla
    {
        private readonly ServicioAccesoDatos _servicioAccesoDatos;

		public ObtenerLinkMultimediaPlantilla(ServicioAccesoDatos servicioAccesoDatos)
		{
			_servicioAccesoDatos = servicioAccesoDatos;
		}
        public async Task<string> ObtenerLinkPlantilla(string idPlantilla)
        {
            try
            {
                // 1. Traer la tabla con los datos reales
                DataTable dtLink = _servicioAccesoDatos.TraerTablaParametros("AccionNodo", "Mensaje", $"IdPlantillaMeta = '{idPlantilla}'");

                // 2. Validar que la consulta contenga al menos una fila
                if (dtLink != null && dtLink.Rows.Count > 0)
                {
                    // 3. Tomar la PRIMERA fila devuelta por la BD (Rows[0]), no una fila nueva
                    DataRow drLink = dtLink.Rows[0];
                    return drLink["Mensaje"]?.ToString() ?? string.Empty;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR ObtenerLinkPlantilla]: {ex.Message}");
                throw;
            }
        }
    }
}
