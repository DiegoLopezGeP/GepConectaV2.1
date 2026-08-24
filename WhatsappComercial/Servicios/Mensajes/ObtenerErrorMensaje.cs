using System.Data;
using WhatsappComercial.Interfaces.Mensajes;
using WhatsappComercial.Modelos.DTOs;
using WhatsappComercial.Servicios.AccesoADatos;

namespace WhatsappComercial.Servicios.Mensajes
{
    public class ObtenerErrorMensaje : IObtenerErrorMensaje
    {
		private readonly ServicioAccesoDatos _servicioAccesoDatos;

		public ObtenerErrorMensaje(ServicioAccesoDatos servicioAccesoDatos)
		{
			_servicioAccesoDatos = servicioAccesoDatos;
		}

        public async Task<ErrorMensaje> ObtenerErrorMensajeSeleccionado(MensajeDTO mensaje)
        {
            try
            {
                string[] parametros = { mensaje.Waid };
                DataTable dtObtenerErrorMensaje = await Task.Run(() => _servicioAccesoDatos.TraerTablaConArreglo(64, parametros));

                // Validar que la consulta devolvió información
                if (dtObtenerErrorMensaje == null || dtObtenerErrorMensaje.Rows.Count == 0)
                {
                    return new ErrorMensaje(); // O lanzar una excepción personalizada según tu lógica
                }

                // Obtener la primera fila devuelta por la base de datos
                DataRow drErrorMensaje = dtObtenerErrorMensaje.Rows[0];

                ErrorMensaje error = new ErrorMensaje()
                {
                    CodigoError = drErrorMensaje["CodigoError"] != DBNull.Value
                        ? Convert.ToInt32(drErrorMensaje["CodigoError"])
                        : 0,
                    TituloError = drErrorMensaje["TituloError"]?.ToString() ?? string.Empty,
                    DetalleError = drErrorMensaje["DetalleError"]?.ToString() ?? string.Empty,
                    ExplicacionError = drErrorMensaje["ExplicacionError"]?.ToString() ?? string.Empty,
                    PosibleSolucion = drErrorMensaje["PosibleSolucion"]?.ToString() ?? string.Empty
                };

                return error;
            }
            catch (Exception ex)
            {
                // Registrar el error según tu sistema de logs antes de re-lanzar
                throw;
            }
        }
    }
}
