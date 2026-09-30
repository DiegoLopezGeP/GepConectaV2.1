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

        public async Task<object?> ObtenerParametrosPlantilla(string idPlantilla, DatosContactoConversacionDTO datosContactoConversacionDTO)
        {
            try
            {
                string[] param = { idPlantilla.ToString() };
                int idConversacion = datosContactoConversacionDTO.IdConversacion;

                // 1. Consultar nodo de la acción
                DataTable dtAccionNodo = await _servicioAccesoDatos.TraerTablaConArregloAsincrono(62, param);

                // Validar si la consulta devolvió filas (Si no, devolvemos un objeto vacío en lugar de null)
                if (dtAccionNodo == null || dtAccionNodo.Rows.Count == 0)
                {
                    return new Dictionary<string, object?>();
                }

                // Tomar la primera fila REAL devuelta por la BD
                DataRow drAccionNodo = dtAccionNodo.Rows[0];

                // Validar si NroConsulta es DBNull o menor/igual a 0 (Significa que la plantilla no necesita parámetros)
                if (drAccionNodo["NroConsulta"] == DBNull.Value || Convert.ToInt32(drAccionNodo["NroConsulta"]) <= 0)
                {
                    return new Dictionary<string, object?>(); // Pasa "como si nada" con un objeto vacío
                }

                // Obtener el número de consulta dinámico
                int nroConsulta = Convert.ToInt32(drAccionNodo["NroConsulta"]);

                // 2. Consultar los parámetros reales de la plantilla
                DataTable dtParametrosPlantilla = await _servicioAccesoDatos.TraerTablaConArregloAsincrono(nroConsulta, [idConversacion.ToString()]);

                // Si la consulta no trae datos, devolvemos objeto vacío
                if (dtParametrosPlantilla == null || dtParametrosPlantilla.Rows.Count == 0)
                {
                    return new Dictionary<string, object?>();
                }

                // 3. Convertir el DataTable a un formato limpio de objeto (Diccionario/Lista de objetos)
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
