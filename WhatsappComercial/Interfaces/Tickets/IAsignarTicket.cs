namespace WhatsappComercial.Interfaces.Tickets
{
    public interface IAsignarTicket
    {
        Task<bool> AsignarTicketNuevaConversacion(int idTicket, int idConversacion);
    }
}
