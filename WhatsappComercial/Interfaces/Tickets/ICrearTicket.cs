using WhatsappComercial.Modelos;
namespace WhatsappComercial.Interfaces.Tickets
{
    public interface ICrearTicket
    {
        Task<int> CrearTicketAsync(Ticket ticket);
    }
}
