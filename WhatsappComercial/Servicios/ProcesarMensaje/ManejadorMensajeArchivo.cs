using GepConecta.WhatsAppCloud.Models.Webhook;
using GepConecta.WhatsAppCloud.Services;
using Microsoft.AspNetCore.Components.Forms;
using WhatsappComercial.Interfaces.Cache;
using WhatsappComercial.Interfaces.Conversaciones;
using WhatsappComercial.Interfaces.GestionArchivos;
using WhatsappComercial.Interfaces.NotificarUI;
using WhatsappComercial.Interfaces.ProcesarMensajeEntrante;
using WhatsappComercial.Modelos;
using WhatsappComercial.Modelos.DTOs;
using WhatsappComercial.Servicios.AccesoADatos;
using WhatsappComercial.Servicios.Cache;

namespace WhatsappComercial.Servicios.ProcesarMensaje
{
    public class ManejadorMensajeArchivo : IManejadorMensajeArchivo
    {
        private readonly ServicioAccesoDatos _servicioAccesoDatos;
        private readonly IGestionArchivos _gestionArchivos;
        private readonly ICacheMensajes _cacheMensajes;
        private readonly INotificarUI _notificadorUI;
        private readonly IServicioValidadorDuplicados _validadorDuplicados;
        private readonly IWhatsAppCloudClient _whatsAppClient;

        // Inyectarías tu cliente de la biblioteca de clases para descargar el archivo usando el Media ID

        public ManejadorMensajeArchivo(ServicioAccesoDatos _servicioAccesoDatos, IGestionArchivos _gestionArchivos,ICacheMensajes _cacheMensajes, INotificarUI _notificadorUI, IServicioValidadorDuplicados _validadorDuplicados ,IWhatsAppCloudClient _whatsAppClient)
        {
            this._servicioAccesoDatos = _servicioAccesoDatos;
            this._cacheMensajes = _cacheMensajes;
            this._notificadorUI = _notificadorUI;
            this._gestionArchivos = _gestionArchivos;
            this._validadorDuplicados = _validadorDuplicados;
            this._whatsAppClient = _whatsAppClient;
        }

        public async Task ProcesarArchivoAsync(WebhookMessage mensajeWA, WebhookContact? contactoWA, string tipo, int idConversacion, string usuario)
        {
            string wamid = mensajeWA.Id ?? string.Empty;

            // 1. FILTRO DE RAM (Previene procesar el mismo webhook repetido por reintentos de Meta)
            if (_validadorDuplicados.EsMensajeDuplicado(wamid))
            {
                Console.WriteLine($"[DUPLICADO EN RAM]: Mensaje multimedia {wamid} ya en proceso. Omitiendo.");
                return;
            }
            _validadorDuplicados.RegistrarMensajeProcesado(wamid);

            // 2. Extraer metadatos según el tipo de archivo recibido
            string mediaId = string.Empty;
            string? caption = string.Empty;
            string? mimeType = string.Empty;
            string? fileName = string.Empty;

            switch (tipo.ToLower())
            {
                case "image":
                    mediaId = mensajeWA.Image?.Id ?? string.Empty;
                    caption = mensajeWA.Image?.Caption;
                    mimeType = mensajeWA.Image?.MimeType ?? "image/jpeg";
                    fileName = $"img_{DateTime.Now:yyyyMMddHHmmssfff}.jpg";
                    break;

                case "audio":
                    mediaId = mensajeWA.Audio?.Id ?? string.Empty;
                    mimeType = mensajeWA.Audio?.MimeType ?? "audio/ogg";
                    fileName = $"audio_{DateTime.Now:yyyyMMddHHmmssfff}.ogg";
                    break;

                case "video":
                    mediaId = mensajeWA.Video?.Id ?? string.Empty;
                    caption = mensajeWA.Video?.Caption;
                    mimeType = mensajeWA.Video?.MimeType ?? "video/mp4";
                    fileName = $"vid_{DateTime.Now:yyyyMMddHHmmssfff}.mp4";
                    break;

                case "document":
                    mediaId = mensajeWA.Document?.Id ?? string.Empty;
                    caption = mensajeWA.Document?.Caption;
                    fileName = mensajeWA.Document?.Filename ?? $"doc_{DateTime.Now:yyyyMMddHHmmssfff}";
                    mimeType = mensajeWA.Document?.MimeType ?? "application/octet-stream";
                    break;

                default:
                    Console.WriteLine($"[WEBHOOK WARNING]: Tipo de mensaje multimedia no reconocido o no soportado: '{tipo}'");
                    break;
            }

            if (string.IsNullOrEmpty(mediaId))
            {
                Console.WriteLine($"[ERROR MULTIMEDIA]: No se encontró MediaID para el mensaje {wamid}");
                return;
            }

            string telefono = mensajeWA.From ?? string.Empty;
            string nombreContacto = contactoWA?.Profile?.Name ?? "Cliente";

            // Homologamos la fecha al reloj local del servidor para mantener orden perfecto
            DateTime fechaMensaje = DateTime.Now;

            string rutaFisicaGuardada = string.Empty;
            string hashSha256 = string.Empty;

            // 3. DESCARGAR DE META Y GUARDAR LOCALMENTE VÍA STREAM
            try
            {
                // A. Descargamos los bytes del archivo desde WhatsApp Graph API
                byte[] bytesDescargados = await _whatsAppClient.DescargarMediaAsync(mediaId);

                if (bytesDescargados != null && bytesDescargados.Length > 0)
                {
                    // B. Convertimos los bytes a Stream en memoria
                    using var streamMeta = new MemoryStream(bytesDescargados);

                    // C. Invocamos tu servicio unificado GuardarStreamYCalcularSha256Async
                    var resultado = await this._gestionArchivos.GuardarStreamYCalcularSha256Async(idConversacion, fileName, mimeType,
                        streamMeta
                    );

                    rutaFisicaGuardada = resultado.RutaRelativa;
                    hashSha256 = resultado.Sha256Hash;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR DESCARGA/GUARDADO MULTIMEDIA]: {ex.Message}");
            }

            // 4. CREAR EL DTO PARA LA UI DE BLAZOR
            var nuevoMensajeUI = new MensajeDTO
            {
                IdMensaje = 0,                       // Temporal mientras responde la BD
                Waid = wamid,                        // Crucial para enlazar eventos de entregado/leído
                Texto = rutaFisicaGuardada ?? string.Empty,     // Texto/leyenda si aplica
                Fecha = fechaMensaje,
                EsEntrante = true,
                TipoArchivo = tipo.ToLower(),
                NombreArchivo = fileName,
                EstadoLectura = "delivered",
                Caption = mensajeWA?.Image?.Caption
            };

            // 5. ACTUALIZAR CACHÉ Y NOTIFICAR A BLAZOR EN TIEMPO REAL
            _cacheMensajes.AgregarMensaje(idConversacion, nuevoMensajeUI);
            await _notificadorUI.NotificarNuevoMensajeEntrante(idConversacion, nuevoMensajeUI);

            // 6. PERSISTIR EL REGISTRO EN LA BASE DE DATOS
            try
            {
                var entidadMensaje = new Modelos.Mensajes
                {
                    IdConversacion = idConversacion,
                    IdMensajeWhatsApp = wamid,
                    ContenidoMensaje = rutaFisicaGuardada ?? string.Empty,
                    FechaEnvioMensaje = fechaMensaje,
                    MensajeEntranteMensaje = true,
                    EsBot = false,
                    NumTelefonoCliente = telefono,
                    NombreUsuarioMensaje = nombreContacto,
                    TipoMensaje = tipo.ToLower(),
                    Sha256Mensaje = hashSha256,
                    Leido = false,
                    MimeTypeMensaje = mimeType,
                    EstadoEnvio = "delivered",
                    Caption = mensajeWA.Image.Caption
                };

                // Insertar en la BD y obtener consecutivo generado
                string idGeneradoStr = this._servicioAccesoDatos.GrabarRegistroDevuelveConsecutivo(entidadMensaje, "Mensajes");


            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR PERSISTENCIA BD ARCHIVO]: {ex.Message}");
            }
        }
    }
}
