using Microsoft.AspNetCore.Components.Forms;

namespace WhatsappComercial.Interfaces.GestionArchivos
{
    public interface IGestionArchivos
    {
        Task<(string RutaRelativa, string Sha256Hash)> GuardarArchivoYCalcularSha256Async(int idTicket, string mimeType, IBrowserFile archivo, long maxFileSize);
        Task<(string RutaRelativa, string Sha256Hash)> GuardarStreamYCalcularSha256Async(int idTicket, string nombreOriginal, string mimeType, Stream streamEntrante);
        string ObtenerTipoArchivo(string fileName);
    }
}
