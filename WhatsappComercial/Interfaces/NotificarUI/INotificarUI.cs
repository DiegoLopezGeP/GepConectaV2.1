using WhatsappComercial.Modelos.DTOs;

namespace WhatsappComercial.Interfaces.NotificarUI
{
    public interface INotificarUI
    {
        /// <summary>
        /// Evento que se dispara cuando entra un nuevo mensaje.
        /// Retorna (idConversacion, nuevoMensajeUI).
        /// </summary>
        event Func<int, MensajeDTO, Task>? OnNuevoMensajeEntrante;
        /// <summary>
        /// Evento que se dispara cuando un mensaje cambia de estado entre send, delivered, read o failed
        /// Retorna (waid, estadoNuevo)
        /// </summary>
        event Func<int, string, string, Task>? CambiodeEstadoMensaje;


        /// <summary>
        /// Método invocado por los manejadores de webhooks para publicar el evento.
        /// </summary>
        Task NotificarNuevoMensajeEntrante(int idConversacion, MensajeDTO nuevoMensajeUI);


        Task NotificarEstadoMensajeCambiado(int idConversacion, string wamid, string nuevoEstado);
    }
}