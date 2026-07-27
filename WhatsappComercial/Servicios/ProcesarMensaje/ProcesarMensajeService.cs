using System.Text.Json;
using GepConecta.WhatsAppCloud.Models.Webhook;
using WhatsappComercial.Interfaces.Conversaciones;
using WhatsappComercial.Interfaces.ProcesarMensajeEntrante;

namespace WhatsappComercial.Servicios.ProcesarMensaje
{
    public class ProcesarMensajeService : IProcesarMensajeEntrante
    {
        private readonly IManejadorMensajeTexto _manejadorTexto;
        private readonly IManejadorMensajeArchivo _manejadorArchivo;
        private readonly IManejadorEstadoLectura _manejadorEstado;
        private readonly IValidarExistenciaConversacion _validarExistenciaConversacion;

        public ProcesarMensajeService(
            IManejadorMensajeTexto manejadorTexto,
            IManejadorMensajeArchivo manejadorArchivo,
            IManejadorEstadoLectura manejadorEstado,
            IValidarExistenciaConversacion validarExistenciaConversacion)
        {
            _manejadorTexto = manejadorTexto;
            _manejadorArchivo = manejadorArchivo;
            _manejadorEstado = manejadorEstado;
            _validarExistenciaConversacion = validarExistenciaConversacion;
        }
        public async Task ProcesarMensajeEntrante(string mensaje, string usuario)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(mensaje)) return;





                // 1. Deserializar el JSON recibido a nuestro DTO principal
                var payload = JsonSerializer.Deserialize<WebhookPayloadWhatsApp>(mensaje);
                var value = payload?.Entry?.FirstOrDefault()?.Changes?.FirstOrDefault()?.Value;

                if (value == null) return;


                int idConversacion = await _validarExistenciaConversacion.ExisteConversacion(value.Messages?.FirstOrDefault()?.From ?? string.Empty);
                // Si no existe la conversación, se puede crear una nueva o manejarlo según la lógica de negocio
                if (idConversacion == 0)
                {
                    //Crear Nueva Conversación // Aqui se manejara la logica de creacion de conversacion por bot y arbol de desición
                }

                // -------------------------------------------------------------------
                // CASO A: VIENE UN MENSAJE ENTRANTE (Texto o Archivo)
                // -------------------------------------------------------------------
                if (value.Messages != null && value.Messages.Any())
                {
                    var mensajeWA = value.Messages.First();
                    var contactoWA = value.Contacts?.FirstOrDefault();

                    string tipo = mensajeWA.Type?.ToLower() ?? "text";

                    switch (tipo)
                    {
                        case "text":
                            await _manejadorTexto.ProcesarTextoAsync(mensajeWA, contactoWA, idConversacion);
                            break;

                        case "image":
                        case "audio":
                        case "document":
                        case "video":
                        case "sticker":
                            await _manejadorArchivo.ProcesarArchivoAsync(mensajeWA, contactoWA, tipo, usuario);
                            break;

                        default:
                            Console.WriteLine($"[WEBHOOK]: Tipo de mensaje no soportado aún: {tipo}");
                            break;
                    }
                }

                // -------------------------------------------------------------------
                // CASO B: VIENE UN CAMBIO DE ESTADO / CONFIRMACIÓN DE LECTURA (Statuses)
                // -------------------------------------------------------------------
                if (value.Statuses != null && value.Statuses.Any())
                {
                    var estadoWA = value.Statuses.First();
                    await _manejadorEstado.ProcesarEstadoAsync(estadoWA, usuario);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR EN PROCESAR MENSAJE ENTRANTE]: {ex.Message}");
            }
        }
    }

}
