using WhatsappComercial.Interfaces.Tickets;
using WhatsappComercial.Servicios.AccesoADatos;

namespace WhatsappComercial.Servicios.Tickets
{
    public class AsignarTicketService : IAsignarTicket
    {
        private readonly ServicioAccesoDatos _servicioAccesoDatos;

        public AsignarTicketService(ServicioAccesoDatos servicioAccesoDatos)
        {
            _servicioAccesoDatos = servicioAccesoDatos;
        }
        public async Task<bool> AsignarTicketNuevaConversacion(int idTicket, int idConversacion)
        {
            if (idTicket <= 0 || idConversacion <= 0)
                return false;

            try
            {
                await Task.Run(() => { _servicioAccesoDatos.ActualizarCampo("Tickets", "IdWhatsapp", idConversacion.ToString(), $"IdTicket = {idTicket}"); });

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
