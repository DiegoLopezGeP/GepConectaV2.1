using WhatsappComercial.Modelos.DTOs;

namespace WhatsappComercial.Interfaces.Mensajes
{
    public interface IObtenerMensajesConversacion
    {
        Task<List<MensajeDTO>> MensajesConversacion(int idConversacion, int cantidad, string? idMensajeCursor);
    }
}
