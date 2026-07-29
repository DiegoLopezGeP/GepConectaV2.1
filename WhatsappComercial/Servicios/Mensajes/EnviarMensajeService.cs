using System.Data;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Components.Forms;
using WhatsappComercial.Interfaces.GestionArchivos;
using WhatsappComercial.Interfaces.Mensajes;
using WhatsappComercial.Modelos;
using WhatsappComercial.Modelos.DTOs;
using WhatsappComercial.Servicios.AccesoADatos;
using WhatsappComercial.Servicios.ConfiguracionEstatica;
using WhatsappComercial.Servicios.SystemServicio;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WhatsappComercial.Servicios.Mensajes
{

    public class EnviarMensajeService : IEnviarMensaje
    {
        private ConfiguracionEstaticaApp _staticConfiguracion = new ConfiguracionEstaticaApp();
        private readonly ServicioAccesoDatos _servicioAccesoDatos;
        private readonly IGestionArchivos _gestionArchivos;




        public EnviarMensajeService(ServicioAccesoDatos servicioAccesoDatos, IGestionArchivos gestionArchivos)
        {
            _servicioAccesoDatos = servicioAccesoDatos;
            _gestionArchivos = gestionArchivos;
        }

        public async Task<int> EnviarMensajeArchivoAsync(int idConversacion, int idTicket, string userName, string? celular, MensajeDTO mensaje, IBrowserFile archivo)
        {
            // Límite de tamaño permitido para el Stream (ej: 15 MB)
            long maxFileSize = 15 * 1024 * 1024;


            // 1. Obtener el MIME Type exacto proveniente del navegador/archivo
            string mimeType = string.IsNullOrWhiteSpace(archivo.ContentType)
                ? "application/octet-stream"
                : archivo.ContentType;

            // 2. Guardar archivo en servidor y calcular SHA256 en un solo flujo
            var (rutaRelativa, sha256Calculado) = await _gestionArchivos.GuardarArchivoYCalcularSha256Async(idTicket, mimeType, archivo, maxFileSize);


            // 3. Determinar la categoría (image, video, audio, document) para la lógica de mensajes
            string TipoMensaje = _gestionArchivos.ObtenerTipoArchivo(archivo.Name);

            // 4. Crear el modelo para la Base de Datos
            var entidadMensaje = new Modelos.Mensajes
            {
                IdConversacion = idConversacion,
                ContenidoMensaje = rutaRelativa,
                FechaEnvioMensaje = mensaje.Fecha,
                MensajeEntranteMensaje = mensaje.EsEntrante,
                EsBot = mensaje.EsEntrante,
                NumTelefonoCliente = celular,
                NombreUsuarioMensaje = userName,
                // Metadata del archivo
                TipoMensaje = TipoMensaje,     // "image", "video", "audio", "document"
                MimeTypeMensaje = mimeType,                 // "image/png", "application/pdf", etc.
                Sha256Mensaje = sha256Calculado,        // Cadena Hash hexadecimal
                Leido = false,
                EstadoEnvio = "send"
            };

            // 5. Guardar en Base de Datos
            string idMensajeRegistrado = _servicioAccesoDatos.GrabarRegistroDevuelveConsecutivo(entidadMensaje, "Mensajes");

            return Convert.ToInt32(idMensajeRegistrado);
        }

        // ==========================================
        // MÉTODOS AUXILIARES PRIVADOS DENTRO DEL SERVICIO
        // ==========================================

        public async Task<int> EnviarMensajeTextoAsync(int idConversacion, string nombreUsuario, string celular, MensajeDTO nuevoMensaje)
        {
            var entidadMensaje = new Modelos.Mensajes
            {
                IdConversacion = idConversacion,
                IdMensajeWhatsApp = nuevoMensaje.Waid,
                ContenidoMensaje = nuevoMensaje.Texto,
                FechaEnvioMensaje = nuevoMensaje.Fecha,
                MensajeEntranteMensaje = nuevoMensaje.EsEntrante,
                EsBot = nuevoMensaje.EsEntrante,
                NumTelefonoCliente = celular,
                NombreUsuarioMensaje = nombreUsuario,
                TipoMensaje = "text",
                Leido = false,
                EstadoEnvio = "send" // Estado inicial: p. ej., "Enviado"
                                     // Asigna los demás campos que requiera tu tabla de BD
            };

            string idMensajeRegistrado = _servicioAccesoDatos.GrabarRegistroDevuelveConsecutivo(entidadMensaje, "Mensajes");

            return Convert.ToInt32(idMensajeRegistrado);

        }


    }
}
