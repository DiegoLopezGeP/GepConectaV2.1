using GepConecta.WhatsAppCloud.Models.Responses;
using Microsoft.AspNetCore.Components.Forms;
using WhatsappComercial.Modelos.DTOs;

namespace WhatsappComercial.Interfaces.Mensajes
{
    public interface IEnviarMensaje
    {
        Task<int> EnviarMensajeTextoAsync(int idConversacion, string nombreUsuario, string celular, MensajeDTO nuevoMensaje);
        Task<int> EnviarMensajeArchivoAsync(int idConversacion, int idTicket, string userName, string? celular, MensajeDTO mensaje, IBrowserFile archivo, EnviarRespuestaWhatsApp respuestaHttp);
    }
}
