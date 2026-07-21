using System.Collections.Concurrent;
using WhatsappComercial.Modelos.DTOs;

namespace WhatsappComercial.Interfaces.Cache
{
    public interface ICacheConversaciones
    {
        /// <summary>
        /// Carga inicial del cache al arrancar la aplicación (warm-up).
        /// Se usa una sola vez, no aplica locks de concurrencia.
        /// </summary>
        void CargarInicial(IEnumerable<DatosTarjetaConversacionDTO> conversaciones);

        /// <summary>
        /// Obtiene todas las conversaciones activas asignadas a un usuario/agente.
        /// </summary>
        Task<List<DatosTarjetaConversacionDTO>> ObtenerActivasPorUsuario(string nombreUsuario);

        /// <summary>
        /// Obtiene todas las conversaciones activas de un área (para bandejas compartidas).
        /// </summary>
        IEnumerable<DatosTarjetaConversacionDTO> ObtenerActivasPorArea(int areaId);

        /// <summary>
        /// Obtiene una conversación puntual por su Id.
        /// </summary>
        DatosTarjetaConversacionDTO? ObtenerPorId(int conversacionId);

        /// <summary>
        /// Crea o actualiza una conversación tanto en BD como en cache de forma atómica.
        /// </summary>
        Task<DatosTarjetaConversacionDTO> CrearOActualizarAsync(DatosTarjetaConversacionDTO conversacion);

        /// <summary>
        /// Actualiza solo el preview del último mensaje y contador de no leídos
        /// (operación frecuente, cada mensaje entrante la dispara).
        /// </summary>
        Task ActualizarUltimoMensajeAsync(int conversacionId, string preview, DateTime fecha, bool incrementarNoLeidos);

        /// <summary>
        /// Marca una conversación como finalizada, persiste en BD y la remueve del cache activo.
        /// </summary>
        Task FinalizarAsync(int conversacionId);

        /// <summary>
        /// Reasigna una conversación a otro usuario (transferencias entre agentes).
        /// </summary>
        Task ReasignarAsync(int conversacionId, int nuevoUsuarioId);

        /// <summary>
        /// Evento que se dispara cada vez que una conversación cambia,
        /// para que los componentes Blazor puedan suscribirse y refrescar su UI.
        /// </summary>
        event Func<DatosTarjetaConversacionDTO, Task>? ConversacionActualizada;

        /// <summary>
        /// Evento que se dispara cuando una conversación se finaliza y sale del cache.
        /// </summary>
        event Func<int, Task>? ConversacionFinalizada;

    }
}
