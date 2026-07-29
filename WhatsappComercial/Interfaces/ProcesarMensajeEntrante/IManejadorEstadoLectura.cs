using GepConecta.WhatsAppCloud.Models.Webhook;

namespace WhatsappComercial.Interfaces.ProcesarMensajeEntrante
{
    public interface IManejadorEstadoLectura
    {
        Task ProcesarEstadoAsync(int idConversacion, WebhookStatus estadoWA, string usuario);
    }
}
