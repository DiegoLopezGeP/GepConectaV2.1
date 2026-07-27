using GepConecta.WhatsAppCloud.Models.Webhook;

namespace WhatsappComercial.Interfaces.ProcesarMensajeEntrante
{
    public interface IManejadorEstadoLectura
    {
        Task ProcesarEstadoAsync(WebhookStatus estadoWA, string usuario);
    }
}
