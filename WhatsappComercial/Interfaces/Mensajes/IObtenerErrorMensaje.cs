using WhatsappComercial.Modelos;
using WhatsappComercial.Modelos.DTOs;

namespace WhatsappComercial.Interfaces.Mensajes
{
    public interface IObtenerErrorMensaje
    {
        Task<ErrorMensaje> ObtenerErrorMensajeSeleccionado(MensajeDTO mensaje);
        Task GuardarLogError(LogsEnvioMensajes logError);
    }
}
