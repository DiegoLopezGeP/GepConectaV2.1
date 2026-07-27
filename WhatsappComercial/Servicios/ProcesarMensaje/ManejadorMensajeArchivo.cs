using GepConecta.WhatsAppCloud.Models.Webhook;
using WhatsappComercial.Interfaces.ProcesarMensajeEntrante;
using WhatsappComercial.Servicios.AccesoADatos;

namespace WhatsappComercial.Servicios.ProcesarMensaje
{
    public class ManejadorMensajeArchivo : IManejadorMensajeArchivo
    {
        private readonly ServicioAccesoDatos _datos;
        // Inyectarías tu cliente de la biblioteca de clases para descargar el archivo usando el Media ID

        public ManejadorMensajeArchivo(ServicioAccesoDatos datos)
        {
            _datos = datos;
        }

        public async Task ProcesarArchivoAsync(WebhookMessage mensajeWA, WebhookContact? contactoWA, string tipo, string usuario)
        {
            string wamid = mensajeWA.Id ?? string.Empty;
            string telefono = mensajeWA.From ?? string.Empty;
            string mediaId = string.Empty;
            string? caption = string.Empty;
            string? mimeType = string.Empty;
            string? fileName = string.Empty;

            // Extraer el Media ID según el tipo de archivo
            switch (tipo)
            {
                case "image":
                    mediaId = mensajeWA.Image?.Id ?? string.Empty;
                    caption = mensajeWA.Image?.Caption;
                    mimeType = mensajeWA.Image?.MimeType;
                    break;
                case "audio":
                    mediaId = mensajeWA.Audio?.Id ?? string.Empty;
                    mimeType = mensajeWA.Audio?.MimeType;
                    break;
                case "document":
                    mediaId = mensajeWA.Document?.Id ?? string.Empty;
                    caption = mensajeWA.Document?.Caption;
                    fileName = mensajeWA.Document?.Filename;
                    mimeType = mensajeWA.Document?.MimeType;
                    break;
            }

            Console.WriteLine($"[ARCHIVO RECIBIDO]: Tipo: {tipo}, MediaID: {mediaId}, Caption: {caption}");

            // TODO: 1. Invocar a WhatsAppClient para descargar los bytes desde Meta
            // TODO: 2. Guardar el archivo localmente en disco/servidor
            // TODO: 3. Insertar el mensaje multimedia en la BD

            await Task.CompletedTask;
        }
    }
}
