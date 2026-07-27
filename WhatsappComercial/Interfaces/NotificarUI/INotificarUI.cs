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
        /// Método invocado por los manejadores de webhooks para publicar el evento.
        /// </summary>
        Task NotificarNuevoMensajeEntrante(int idConversacion, MensajeDTO nuevoMensajeUI);
    }
}