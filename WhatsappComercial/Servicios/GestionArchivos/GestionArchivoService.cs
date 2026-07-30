using System.Security.Cryptography;
using Microsoft.AspNetCore.Components.Forms;
using WhatsappComercial.Interfaces.GestionArchivos;
using WhatsappComercial.Servicios.AccesoADatos;
using WhatsappComercial.Servicios.ConfiguracionEstatica;
using WhatsappComercial.Servicios.SystemServicio;

namespace WhatsappComercial.Servicios.GestionArchivos
{
    public class GestionArchivoService : IGestionArchivos
    {
        private ConfiguracionEstaticaApp _staticConfiguracion = new ConfiguracionEstaticaApp();
        private readonly ServicioAccesoDatos _servicioAccesoDatos;
        private readonly SystemService _systemServicio;

        public GestionArchivoService(ServicioAccesoDatos servicioAccesoDatos, SystemService systemServicio)
        {
            _servicioAccesoDatos = servicioAccesoDatos;
            _systemServicio = systemServicio;
        }



        public async Task<(string RutaRelativa, string Sha256Hash)> GuardarArchivoYCalcularSha256Async(int idTicket, string mimeType, IBrowserFile archivo, long maxFileSize)
        {
            // Abrimos el stream de Blazor y reutilizamos la lógica unificada
            await using var browserStream = archivo.OpenReadStream(maxFileSize);

            return await GuardarStreamYCalcularSha256Async( idTicket, archivo.Name,  mimeType, browserStream);
        }

        public string ObtenerTipoArchivo(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return "unknown";

            string extension = System.IO.Path.GetExtension(fileName).ToLower();

            return extension switch
            {
                // Imágenes
                ".jpeg" or ".jpg" or ".png" or ".gif" or ".bmp" or ".webp" or ".svg" or ".ico" or ".tiff" => "image",

                // Videos
                ".mp4" or ".avi" or ".mov" or ".mkv" or ".flv" or ".wmv" or ".m4v" or ".3gp" => "video",

                // Audios (Incluyendo .webm y .opus provenientes del grabador de voz)
                ".mp3" or ".wav" or ".m4a" or ".ogg" or ".opus" or ".webm" or ".aac" or ".wma" or ".flac" => "audio",

                // Documentos
                ".pdf" or ".doc" or ".docx" or ".xls" or ".xlsx" or ".ppt" or ".pptx" or ".txt" or ".csv" or ".rtf" => "document",

                // Comprimidos (Opcional)
                ".zip" or ".rar" or ".7z" or ".tar" or ".gz" => "compressed",

                _ => "unknown"
            };
        }

        public async Task<(string RutaRelativa, string Sha256Hash)> GuardarStreamYCalcularSha256Async(int idTicket, string nombreOriginal, string mimeType, Stream streamEntrante)
        {
            string carpetaUploads = _staticConfiguracion.TraerConfiguracionPorCondicion("CarpetaMultimedia");

            // 1. Obtener el tipo de archivo ("document", "image", etc.)
            string tipoArchivo = ObtenerTipoArchivo(nombreOriginal);

            // 2. Construir la carpeta final
            string carpetaFinalDestino = Path.Combine(_systemServicio.PathDocument(), carpetaUploads, tipoArchivo);

            // 3. Crear la carpeta final si no existe
            if (!Directory.Exists(carpetaFinalDestino))
            {
                Directory.CreateDirectory(carpetaFinalDestino);
            }

            // 4. Construir la ruta física completa del archivo
            string extension = Path.GetExtension(nombreOriginal);
            string nombreUnico = $"{idTicket}_{DateTime.Now:yyyyMMddHHmmssfff}{extension}";
            string rutaFisicaCompleta = Path.Combine(carpetaFinalDestino, nombreUnico);

            string sha256Hash = string.Empty;

            // 5. Instanciar algoritmo y procesar streams
            using (var sha256 = SHA256.Create())
            {
                await using (var fileStream = new FileStream(rutaFisicaCompleta, FileMode.Create, FileAccess.Write))
                await using (var cryptoStream = new CryptoStream(fileStream, sha256, CryptoStreamMode.Write))
                {
                    // Copiar datos del stream de origen (ya sea de WhatsApp o de Blazor)
                    await streamEntrante.CopyToAsync(cryptoStream);
                    await cryptoStream.FlushFinalBlockAsync();
                }

                // 6. Convertir el hash
                sha256Hash = Convert.ToHexString(sha256.Hash!).ToLowerInvariant();
            }

            return (rutaFisicaCompleta, sha256Hash);
        }
    }
}
