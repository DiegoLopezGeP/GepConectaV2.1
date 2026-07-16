using WhatsappComercial.Interfaces.Tickets;
using WhatsappComercial.Modelos;
using WhatsappComercial.Servicios.AccesoADatos;

namespace WhatsappComercial.Servicios.Tickets
{
    public class CrearTicketService : ICrearTicket
    {
        private readonly ServicioAccesoDatos _servicioAccesoDatos;

        public CrearTicketService(ServicioAccesoDatos servicioAccesoDatos)
        {
            _servicioAccesoDatos = servicioAccesoDatos;
        }
        public async Task<int> CrearTicketAsync(Ticket ticket)
        {
            try
            {
                string idTicketRegistrado = _servicioAccesoDatos.GrabarRegistroDevuelveConsecutivo(ticket, "Tickets");

                return int.TryParse(idTicketRegistrado, out int idTicket)
                    ? idTicket
                    : 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
