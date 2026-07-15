using WhatsappComercial.Modelos;
namespace WhatsappComercial.Interfaces.Tickets
{
    public interface ICrearTicket
    {
        Task<int> CrearTicket(Ticket ticket);
    }
}
