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
            string carpetaUploads = _staticConfiguracion.TraerConfiguracionPorCondicion("CarpetaMultimedia");

            // 1. Obtener el tipo de archivo ("document", "image", etc.)
            string tipoArchivo = ObtenerTipoArchivo(archivo.Name);

            // 2. Construir la carpeta final (incluyendo la subcarpeta "document")
            string carpetaFinalDestino = Path.Combine(_systemServicio.PathDocument(), carpetaUploads, tipoArchivo);

            // 3. ¡AQUÍ ESTÁ LA CLAVE! Crear la carpeta final si no existe
            if (!Directory.Exists(carpetaFinalDestino))
            {
                Directory.CreateDirectory(carpetaFinalDestino);
            }

            // 4. Construir la ruta física completa del archivo
            string extension = Path.GetExtension(archivo.Name);
            string nombreUnico = $"{idTicket}_{archivo.Name}";
            string rutaFisicaCompleta = Path.Combine(carpetaFinalDestino, nombreUnico);

            string sha256Hash = string.Empty;

            // 5. Instanciar algoritmo y procesar streams
            using (var sha256 = SHA256.Create())
            {
                await using (var browserStream = archivo.OpenReadStream(maxFileSize))
                await using (var fileStream = new FileStream(rutaFisicaCompleta, FileMode.Create, FileAccess.Write))
                await using (var cryptoStream = new CryptoStream(fileStream, sha256, CryptoStreamMode.Write))
                {
                    // Copiar datos del navegador hacia el archivo en disco procesando el hash
                    await browserStream.CopyToAsync(cryptoStream);
                    await cryptoStream.FlushFinalBlockAsync();
                }
                // AL SALIR DE ESTE BLOQUE SE CIERRAN Y VACÍAN LOS STREAMS EN DISCO

                // 6. Convertir el hash de bytes a cadena Hexadecimal minúscula
                sha256Hash = Convert.ToHexString(sha256.Hash!).ToLowerInvariant();
            }

            return (rutaFisicaCompleta, sha256Hash);
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
    }
}
