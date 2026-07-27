using GepConecta.WhatsAppCloud.Models.Webhook;

namespace WhatsappComercial.Interfaces.ProcesarMensajeEntrante
{
    public interface IManejadorMensajeArchivo
    {
        Task ProcesarArchivoAsync(WebhookMessage mensajeWA, WebhookContact? contactoWA, string tipo, string usuario);
    }
}
