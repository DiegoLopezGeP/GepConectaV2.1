using System.Collections.Concurrent;
using WhatsappComercial.Interfaces.Cache;
using WhatsappComercial.Modelos.DTOs;

namespace WhatsappComercial.Servicios.Cache
{
    public class ContenedorConversacionCache
    {
        public List<MensajeDTO> Mensajes { get; } = new();
        public bool SeCargaronTodosLosAntiguos { get; set; } = false;
        public object CandadoInterno { get; } = new();
    }

    public class CacheMensajes : ICacheMensajes
    {
        private readonly int _capacidadMaximaConversaciones;
        private readonly object _bloqueoOrden = new();

        private readonly ConcurrentDictionary<int, ContenedorConversacionCache> _cache = new();
        private readonly LinkedList<int> _ordenUso = new();
        private readonly Dictionary<int, LinkedListNode<int>> _nodosPorConversacion = new();

        public CacheMensajes(int capacidadMaximaConversaciones = 50)
        {
            _capacidadMaximaConversaciones = capacidadMaximaConversaciones;
        }

        public List<MensajeDTO>? ObtenerDeCache(int idConversacion)
        {
            if (_cache.TryGetValue(idConversacion, out var contenedor))
            {
                MarcarComoUsadaRecientemente(idConversacion);

                // Hacemos una copia thread-safe para que la UI de Blazor no colisione con SignalR
                lock (contenedor.CandadoInterno)
                {
                    return contenedor.Mensajes.ToList();
                }
            }

            return null;
        }

        public void GuardarEnCache(int idConversacion, List<MensajeDTO> mensajes)
        {
            var contenedor = _cache.GetOrAdd(idConversacion, _ => new ContenedorConversacionCache());

            lock (contenedor.CandadoInterno)
            {
                contenedor.Mensajes.Clear();
                contenedor.Mensajes.AddRange(mensajes);
            }

            MarcarComoUsadaRecientemente(idConversacion);
            AplicarLimiteCapacidad();
        }

        public void AgregarMensaje(int idConversacion, MensajeDTO mensaje)
        {
            if (_cache.TryGetValue(idConversacion, out var contenedor))
            {
                lock (contenedor.CandadoInterno)
                {
                    // Evitar duplicados si SignalR y el envío local reaccionan al mismo tiempo
                    if (!contenedor.Mensajes.Any(m => m.IdMensaje == mensaje.IdMensaje && m.IdMensaje > 0))
                    {
                        contenedor.Mensajes.Add(mensaje);
                    }
                }
                MarcarComoUsadaRecientemente(idConversacion);
            }
        }

        public void AgregarMensajesAntiguos(int idConversacion, List<MensajeDTO> mensajesAntiguos)
        {
            if (_cache.TryGetValue(idConversacion, out var contenedor))
            {
                lock (contenedor.CandadoInterno)
                {
                    contenedor.Mensajes.InsertRange(0, mensajesAntiguos);
                }
            }
        }

        public void RemoverDeCache(int idConversacion)
        {
            _cache.TryRemove(idConversacion, out _);

            lock (_bloqueoOrden)
            {
                if (_nodosPorConversacion.TryGetValue(idConversacion, out var nodo))
                {
                    _ordenUso.Remove(nodo);
                    _nodosPorConversacion.Remove(idConversacion);
                }
            }
        }

        private void MarcarComoUsadaRecientemente(int idConversacion)
        {
            lock (_bloqueoOrden)
            {
                if (_nodosPorConversacion.TryGetValue(idConversacion, out var nodoExistente))
                {
                    _ordenUso.Remove(nodoExistente);
                }

                var nuevoNodo = _ordenUso.AddFirst(idConversacion);
                _nodosPorConversacion[idConversacion] = nuevoNodo;
            }
        }

        private void AplicarLimiteCapacidad()
        {
            lock (_bloqueoOrden)
            {
                while (_ordenUso.Count > _capacidadMaximaConversaciones)
                {
                    var idMenosReciente = _ordenUso.Last!.Value;
                    _ordenUso.RemoveLast();
                    _nodosPorConversacion.Remove(idMenosReciente);
                    _cache.TryRemove(idMenosReciente, out _);
                }
            }
        }
    }
}