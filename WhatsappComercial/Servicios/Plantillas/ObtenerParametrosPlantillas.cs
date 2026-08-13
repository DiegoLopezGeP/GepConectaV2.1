using System.Data;
using WhatsappComercial.Interfaces.Plantillas;
using WhatsappComercial.Modelos.DTOs;
using WhatsappComercial.Servicios.AccesoADatos;

namespace WhatsappComercial.Servicios.Plantillas
{
    public class ObtenerParametrosPlantillas : IObtenerParametrosPlantilla
    {
        private readonly ServicioAccesoDatos _servicioAccesoDatos;
        public ObtenerParametrosPlantillas(ServicioAccesoDatos servicioAccesoDatos)
        {
            _servicioAccesoDatos = servicioAccesoDatos;
        }

        public async Task<object?> ObtenerParametrosPlantilla(string idPlantilla, DatosTarjetaConversacionDTO datosTarjetaConversacionDTO)
        {
            try
            {
                string[] param = { idPlantilla.ToString() };
                int idConversacion = datosTarjetaConversacionDTO.IdConversacion;
                // 1. Consultar nodo de la acción
                DataTable dtAccionNodo = await _servicioAccesoDatos.TraerTablaConArregloAsincrono(62, param);

                // Validar si la consulta devolvió filas
                if (dtAccionNodo == null || dtAccionNodo.Rows.Count == 0)
                {
                    return null;
                }

                // Tomar la primera fila REAL devuelta por la BD
                DataRow drAccionNodo = dtAccionNodo.Rows[0];

                // Obtener el número de consulta dinámico
                int nroConsulta = Convert.ToInt32(drAccionNodo["NroConsulta"]);

                // 2. Consultar los parámetros reales de la plantilla
                DataTable dtParametrosPlantilla = await _servicioAccesoDatos.TraerTablaConArregloAsincrono(nroConsulta, [idConversacion.ToString()]);

                if (dtParametrosPlantilla == null || dtParametrosPlantilla.Rows.Count == 0)
                {
                    return null;
                }

                // 3. Convertir el DataTable a un formato limpio de objeto (Diccionario/Lista de objetos)
                // Esto permite que se serialice perfectamente a JSON o C# dinámico
                var listaParametros = new List<Dictionary<string, object?>>();

                foreach (DataRow row in dtParametrosPlantilla.Rows)
                {
                    var filaDiccionario = new Dictionary<string, object?>();
                    foreach (DataColumn col in dtParametrosPlantilla.Columns)
                    {
                        filaDiccionario[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                    }
                    listaParametros.Add(filaDiccionario);
                }

                // Si la consulta devuelve una sola fila, devolvemos el objeto directo. 
                // Si devuelve varias filas, devolvemos la lista completa.
                return listaParametros.Count == 1 ? listaParametros[0] : listaParametros;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR ObtenerParametrosPlantilla]: {ex.Message}");
                throw;
            }
        }
    }
}
