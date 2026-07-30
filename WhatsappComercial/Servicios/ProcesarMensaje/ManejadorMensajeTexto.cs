using GepConecta.WhatsAppCloud.Models.Webhook;
using WhatsappComercial.Interfaces.Cache;
using WhatsappComercial.Interfaces.ProcesarMensajeEntrante;
using WhatsappComercial.Interfaces.Tickets;
using WhatsappComercial.Modelos;
using WhatsappComercial.Modelos.DTOs;
using WhatsappComercial.Servicios.AccesoADatos;
using WhatsappComercial.Interfaces.NotificarUI;

namespace WhatsappComercial.Servicios.ProcesarMensaje
{
    public class ManejadorMensajeTexto : IManejadorMensajeTexto
    {
        private readonly ServicioAccesoDatos _datos;
        private readonly ICacheMensajes _cacheMensajes;
        private readonly ICrearTicket _crearTicket;
        private readonly INotificarUI _notificadorUI;
        private readonly IServicioValidadorDuplicados _validadorDuplicados;

        public ManejadorMensajeTexto(ServicioAccesoDatos datos, ICacheMensajes cacheMensajes, ICrearTicket crearTicket, INotificarUI notificadorUI, IServicioValidadorDuplicados validadorDuplicados)
        {
            _datos = datos;
            _cacheMensajes = cacheMensajes;
            _notificadorUI = notificadorUI;
            _validadorDuplicados = validadorDuplicados;
        }

        public async Task ProcesarTextoAsync(WebhookMessage mensajeWA, WebhookContact? contactoWA, int idConversacion)
        {
            string wamid = mensajeWA.Id ?? string.Empty;

            // 1. FILTRO DE RAM (Para atajar reintentos inmediatos de Meta en microsegundos)
            if (_validadorDuplicados.EsMensajeDuplicado(wamid))
            {
                Console.WriteLine($"[DUPLICADO EN RAM]: Mensaje {wamid} ya en proceso. Omitiendo.");
                return;
            }

            // Marca en RAM inmediatamente para bloquear ráfagas concurrentes de la misma llamada
            _validadorDuplicados.RegistrarMensajeProcesado(wamid);

            // 2. PROCESAR Y ACTUALIZAR LA CACHÉ DE LA INTERFAZ BLAZOR (Respuesta Ultra Rápida)
            string telefono = mensajeWA.From ?? string.Empty;
            string nombreContacto = contactoWA?.Profile?.Name ?? "Cliente";
            string contenidoTexto = mensajeWA.Text?.Body ?? string.Empty;
            //DateTime fechaMensaje = DateTimeOffset.FromUnixTimeSeconds(long.Parse(mensajeWA.Timestamp ?? "0")).LocalDateTime;
            DateTime fechaMensaje = DateTime.Now;
            // B. Crear objeto DTO para la pantalla (Sin WAID)
            var nuevoMensajeUI = new MensajeDTO
            {
                Texto = contenidoTexto,
                Fecha = fechaMensaje,
                EsEntrante = true,

            };

            // C. Agregar a la Caché UI y notificar a Blazor
            _cacheMensajes.AgregarMensaje(idConversacion, nuevoMensajeUI);
            await _notificadorUI.NotificarNuevoMensajeEntrante(idConversacion, nuevoMensajeUI);

            // 3. PERSISTIR EN BASE DE DATOS (Delegando la validación de duplicados al INSERT)
            try
            {
                var entidadMensaje = new Modelos.Mensajes
                {
                    IdConversacion = idConversacion,
                    IdMensajeWhatsApp = wamid,
                    ContenidoMensaje = contenidoTexto,
                    FechaEnvioMensaje = fechaMensaje,
                    MensajeEntranteMensaje = true,
                    EsBot = true,
                    NumTelefonoCliente = telefono,
                    NombreUsuarioMensaje = nombreContacto,
                    TipoMensaje = "text",
                    Leido = false,
                    EstadoEnvio = "send" // Estado inicial: p. ej., "Enviado"
                                         // Asigna los demás campos que requiera tu tabla de BD
                };

                // 5. Guardar en Base de Datos
                string idMensajeRegistrado = _datos.GrabarRegistroDevuelveConsecutivo(entidadMensaje, "Mensajes");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR PERSISTENCIA BD]: {ex.Message}");
            }
        }
    }
}
