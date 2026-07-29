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


        public ManejadorEstadoLectura(ServicioAccesoDatos datos, ICacheMensajes cacheMensajes, IModificarEstadoMensaje modificarEstadoMensaje, INotificarUI notificarUI, IColaEstadoMensajes colaEstadoMensajes)
        {
            _cacheMensajes = cacheMensajes;
            _modificarEstadoMensaje = modificarEstadoMensaje;
            _notificarUI = notificarUI;
            _colaEstadoMensajes = colaEstadoMensajes;
        }

        public async Task ProcesarEstadoAsync(int idConversacion, WebhookStatus estadoWA, string usuario)
        {
            string wamid = estadoWA.Id ?? string.Empty;
            string estado = estadoWA.Status ?? string.Empty; // "sent", "delivered", "read", "failed"

            if (string.IsNullOrEmpty(wamid)) return;

            // 1. Actualización en Cache (retorna true si realmente cambió el estado)
            bool seActualizoCache = await _cacheMensajes.ActualizarEstadoPorWaidAsync(wamid, estado);

            // 2. Si la caché cambió, notificamos a la UI para que Blazor vuelva a renderizar
            if (seActualizoCache)
            {
                await _notificarUI.NotificarEstadoMensajeCambiado(idConversacion, wamid, estado);
            }

            // 3. Encolar para la Base de Datos
            _colaEstadoMensajes.EncolarEstado(new EstadoMensajeItem
            {
                IdConversacion = idConversacion,
                IdMensajeWhatsApp = wamid,
                EstadoEnvio = estado,
            });

            Console.WriteLine($"[CONFIRMACIÓN LECTURA]: El mensaje {wamid} cambió a estado '{estado}'");
        }
    }
}
