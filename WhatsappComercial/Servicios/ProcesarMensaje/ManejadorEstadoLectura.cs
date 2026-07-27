using GepConecta.WhatsAppCloud.Models.Webhook;
using WhatsappComercial.Interfaces.ProcesarMensajeEntrante;
using WhatsappComercial.Servicios.AccesoADatos;

namespace WhatsappComercial.Servicios.ProcesarMensaje
{
    public class ManejadorEstadoLectura : IManejadorEstadoLectura
    {
        private readonly ServicioAccesoDatos _datos;

        public ManejadorEstadoLectura(ServicioAccesoDatos datos)
        {
            _datos = datos;
        }

        public async Task ProcesarEstadoAsync(WebhookStatus estadoWA, string usuario)
        {
            string wamid = estadoWA.Id ?? string.Empty;
            string estado = estadoWA.Status ?? string.Empty; // "sent", "delivered", "read", "failed"

            // Convertir timestamp
            DateTime fechaEstado = DateTimeOffset.FromUnixTimeSeconds(long.Parse(estadoWA.Timestamp ?? "0")).LocalDateTime;

            // TODO: Aquí ejecutas la actualización en la base de datos de tu tabla 'Mensajes'
            // Ejemplo: UPDATE Mensajes SET Estado = @estado WHERE WaMessageId = @wamid
            Console.WriteLine($"[CONFIRMACIÓN LECTURA]: El mensaje {wamid} cambió a estado '{estado}' el {fechaEstado}");

            await Task.CompletedTask;
        }
    }
}
