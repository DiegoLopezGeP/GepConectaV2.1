using WhatsappComercial.Interfaces.Mensajes;
using WhatsappComercial.Servicios.AccesoADatos;

namespace WhatsappComercial.Servicios.Mensajes
{
    public class ModificarEstadoMensaje : IModificarEstadoMensaje
    {
		private readonly ServicioAccesoDatos _servicioAccesoDatos;

		public ModificarEstadoMensaje(ServicioAccesoDatos servicioAccesoDatos)
		{
			_servicioAccesoDatos = servicioAccesoDatos;
		}

        public async Task ActualizarEstadoLecturaMensajeEntrante(int idConversacion)
        {
            try
            {
                _servicioAccesoDatos.ActualizarCampo("Mensajes", "Leido", $"{1}", $"IdConversacion = {idConversacion}");
            }
            catch (Exception ex)
            {

                throw;
            }
            ;
        }

        public async Task ActualizarEstadoMensaje(int idConversacion, string waid, string nuevoEstado)
        {
			try
			{
				_servicioAccesoDatos.ActualizarCampo("Mensajes", "EstadoEnvio", $"{nuevoEstado}", $"IdConversacion = {idConversacion} and IdMensajeWhatsApp = '{waid}'");
            }
			catch (Exception ex)
			{

				throw;
			}
			;
        }
    }
}
