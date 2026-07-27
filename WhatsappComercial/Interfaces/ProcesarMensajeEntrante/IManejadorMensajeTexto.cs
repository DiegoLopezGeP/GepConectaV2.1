using GepConecta.WhatsAppCloud.Models.Webhook;

namespace WhatsappComercial.Interfaces.ProcesarMensajeEntrante
{
    public interface IManejadorMensajeTexto
    {
        Task ProcesarTextoAsync(WebhookMessage mensajeWA, WebhookContact? contactoWA, int idConversacion);
    }
}
