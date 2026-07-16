using WhatsappComercial.Interfaces.Conversaciones;
using WhatsappComercial.Modelos;
using WhatsappComercial.Servicios.AccesoADatos;

namespace WhatsappComercial.Servicios.Conversaciones
{
    public class CrearConversacionService : ICrearConversacion
    {
        private readonly ServicioAccesoDatos _servicioAccesoDatos;

        public CrearConversacionService(ServicioAccesoDatos servicioAccesoDatos)
        {
            _servicioAccesoDatos = servicioAccesoDatos;
        }
        public async Task<int> CrearNuevaConversaciones(Conversacion conversacion)
        {
			string idConversacion = string.Empty;
            try
			{
                idConversacion = _servicioAccesoDatos.GrabarRegistroDevuelveConsecutivo(conversacion, "Conversaciones");

                return int.TryParse(idConversacion, out int idTicket)
                   ? idTicket
                   : 0;
            }
			catch (Exception ex)
			{
                Console.WriteLine(ex.Message);
				throw;
			}
        }
    }
}
