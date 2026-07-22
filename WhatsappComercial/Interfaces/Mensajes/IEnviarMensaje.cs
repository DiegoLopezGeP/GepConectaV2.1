using WhatsappComercial.Modelos.DTOs;

namespace WhatsappComercial.Interfaces.Mensajes
{
    public interface IEnviarMensaje
    {
        Task<int> EnviarMensajeTextoAsync(int idConversacion, string nombreUsuario, string celular, MensajeDTO nuevoMensaje);
    }
}
