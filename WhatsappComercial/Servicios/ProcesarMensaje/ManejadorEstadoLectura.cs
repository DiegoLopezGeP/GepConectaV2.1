using GepConecta.WhatsAppCloud.Models.Webhook;
using WhatsappComercial.Interfaces.Cache;
using WhatsappComercial.Interfaces.Mensajes;
using WhatsappComercial.Interfaces.NotificarUI;
using WhatsappComercial.Interfaces.ProcesarMensajeEntrante;
using WhatsappComercial.Modelos;
using WhatsappComercial.Modelos.DTOs;
using WhatsappComercial.Servicios.AccesoADatos;

using WhatsappComercial.Servicios.Cache;

namespace WhatsappComercial.Servicios.ProcesarMensaje
{
    public class ManejadorEstadoLectura : IManejadorEstadoLectura
    {
        private readonly ICacheMensajes _cacheMensajes;
        private readonly IModificarEstadoMensaje _modificarEstadoMensaje;
        private readonly INotificarUI _notificarUI;
        private readonly IColaEstadoMensajes _colaEstadoMensajes;
        private readonly IColaLogsEnvio _colaLogsEnvio;


        public ManejadorEstadoLectura(ServicioAccesoDatos datos, ICacheMensajes cacheMensajes, IModificarEstadoMensaje modificarEstadoMensaje, INotificarUI notificarUI, IColaEstadoMensajes colaEstadoMensajes, IColaLogsEnvio colaLogsEnvio)
        {
            _cacheMensajes = cacheMensajes;
            _modificarEstadoMensaje = modificarEstadoMensaje;
            _notificarUI = notificarUI;
            _colaEstadoMensajes = colaEstadoMensajes;
            _colaLogsEnvio = colaLogsEnvio;
        }

        public async Task ProcesarEstadoAsync(int idConversacion, WebhookStatus estadoWA, string usuario)
        {
            string wamid = estadoWA.Id ?? string.Empty;
            string estado = estadoWA.Status ?? string.Empty; // "sent", "delivered", "read", "failed"

            if (string.IsNullOrEmpty(wamid)) return;

            // 1. Actualización en Cache
            bool seActualizoCache = await _cacheMensajes.ActualizarEstadoPorWaidAsync(wamid, estado);

            // 2. Notificar a la UI si hubo cambios
            if (seActualizoCache)
            {
                await _notificarUI.NotificarEstadoMensajeCambiado(idConversacion, wamid, estado);
            }

            // 3. Encolar para la Base de Datos (Actualiza 'Mensajes')
            _colaEstadoMensajes.EncolarEstado(new EstadoMensajeItem
            {
                IdConversacion = idConversacion,
                IdMensajeWhatsApp = wamid,
                EstadoEnvio = estado,
            });

            // 4. Mapeo y encolado del log de errores cuando falla (Inserta en 'LogsEnvioMensajes')
            if (estado == "failed")
            {
                var primerError = estadoWA.Errors?.FirstOrDefault();

                DateTime fechaEstado = DateTime.UtcNow;
                if (long.TryParse(estadoWA.Timestamp, out long unixTime))
                {
                    fechaEstado = DateTimeOffset.FromUnixTimeSeconds(unixTime).LocalDateTime;
                }

                var logError = new LogsEnvioMensajes
                {
                    CodigoError = primerError?.Code ?? 0,
                    IdConversacion = idConversacion,
                    IdMensajeWhatsApp = wamid,
                    DetalleError = primerError?.Message ?? "Error no especificado",
                    FechaEstado = fechaEstado,
                    FechaRegistroLog = DateTime.Now,
                    UsuarioEnvioMensaje = usuario,
                    NumeroReceptor = estadoWA.RecipientId ?? string.Empty
                };

                // ENCOLAR LOG PARA QUE EL WORKER LO GUARDE EN BD
                _colaLogsEnvio.EncolarLog(logError);
            }

            Console.WriteLine($"[CONFIRMACIÓN LECTURA]: El mensaje {wamid} cambió a estado '{estado}'");
        }
    }
}
