using System.Data;
using WhatsappComercial.Interfaces.Plantillas;
using WhatsappComercial.Modelos.DTOs;
using WhatsappComercial.Servicios.AccesoADatos;

namespace WhatsappComercial.Servicios.Plantillas
{
    public class ObtenerPlantillas : IObtenerPlantillas
    {
		private readonly ServicioAccesoDatos _servicioAccesoDatos;
		public ObtenerPlantillas(ServicioAccesoDatos servicioAccesoDatos)
		{
			_servicioAccesoDatos = servicioAccesoDatos;
		}
        public async Task<List<PlantillaDTO>> ObtenerPlantillasActivas(string[] idGrupoTrabajo)
        {
            try
            {
                // 1. Instanciación correcta de la lista
                List<PlantillaDTO> plantillas = new List<PlantillaDTO>();

                // 2. Obtener los datos (se usa await si tu método TraerTablaNombre admite llamadas asíncronas)
                DataTable dtPlantillas = await _servicioAccesoDatos.TraerTablaConArregloAsincrono(61, idGrupoTrabajo);

                // 3. Recorrer las filas de la tabla
                if (dtPlantillas != null && dtPlantillas.Rows.Count > 0)
                {
                    foreach (DataRow row in dtPlantillas.Rows)
                    {
                        // Crear un nuevo objeto por cada fila
                        var plantilla = new PlantillaDTO()
                        {
                            Id = row["IdPlantillaMeta"].ToString() ?? string.Empty,
                            NombrePlantilla = row["nombrePlantilla"]?.ToString()?.Replace("_", " ") ?? string.Empty,
                        };

                        // Agregar a la lista
                        plantillas.Add(plantilla);
                    }
                }

                return plantillas;
            }
            catch (Exception ex)
            {
                // Loguear la excepción si es necesario antes de relanzarla
                Console.WriteLine($"Error al obtener plantillas activas: {ex.Message}");
                throw;
            }
        }
    }
}
