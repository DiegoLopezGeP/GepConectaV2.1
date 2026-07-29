using WhatsappComercial.Modelos.DTOs;
using static WhatsappComercial.Components.ComponentesHijo.Conversaciones.VisorMensajes;

namespace WhatsappComercial.Interfaces.Cache
{
    public interface ICacheMensajes
    {
        /// <summary>
        /// Intenta obtener los mensajes ya cacheados de una conversación.
        /// Si no están en cache, retorna null (el llamador decide ir a BD).
        /// </summary>
        List<MensajeDTO>? ObtenerDeCache(int idConversacion);

        /// <summary>
        /// Guarda o reemplaza los mensajes de una conversación en el cache,
        /// aplicando política LRU si se supera la capacidad máxima.
        /// </summary>
        void GuardarEnCache(int idConversacion, List<MensajeDTO> mensajes);

        /// <summary>
        /// Agrega un mensaje nuevo al final de una conversación ya cacheada
        /// (usado cuando llega un mensaje en tiempo real vía webhook/SignalR).
        /// Si la conversación no está en cache, no hace nada (se cargará
        /// completa la próxima vez que se abra).
        /// </summary>
        void AgregarMensaje(int idConversacion, MensajeDTO mensaje);

        /// <summary>
        /// Inserta mensajes más antiguos al INICIO del cache existente
        /// (usado al paginar hacia atrás con scroll).
        /// </summary>
        void AgregarMensajesAntiguos(int idConversacion, List<MensajeDTO> mensajesAntiguos);

        /// <summary>
        /// Elimina una conversación del cache (ej. al finalizarla).
        /// </summary>
        void RemoverDeCache(int idConversacion);

        /// <summary>
        /// Actualiza el estado del mensaje en cache
        /// </summary>
        /// <param name="waid"></param>
        /// <param name="nuevoEstado"></param>
        /// <returns></returns>
        Task<bool> ActualizarEstadoPorWaidAsync(string waid, string nuevoEstado);

    }
}
