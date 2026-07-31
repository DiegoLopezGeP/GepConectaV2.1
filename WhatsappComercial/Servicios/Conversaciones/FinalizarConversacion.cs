using WhatsappComercial.Enums;
using WhatsappComercial.Interfaces.Conversaciones;
using WhatsappComercial.Servicios.AccesoADatos;

namespace WhatsappComercial.Servicios.Conversaciones
{
    public class FinalizarConversacion : IFinalizarConversacion
    {
		private readonly ServicioAccesoDatos _servicioAccesoDatos;

		public FinalizarConversacion(ServicioAccesoDatos servicioAccesoDatos)
		{
			_servicioAccesoDatos = servicioAccesoDatos;

        }
        public async Task FinalizarConversacionManual(int idConversacion, EstadoConversacionEnum estadoFinalizado)
        {
			try
			{
                string fechaFormateada = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                _servicioAccesoDatos.ActualizarCampo("Conversaciones", "EstadoConversacion", $"'{estadoFinalizado.ToString()}'", $"Idconversacion = {idConversacion}");
				_servicioAccesoDatos.ActualizarCampo("Conversaciones", "FechaFinConversacion", $"'{fechaFormateada}'", $"Idconversacion = {idConversacion}");
            }
			catch (Exception ex)
			{

				throw;
			}
        }
    }
}
