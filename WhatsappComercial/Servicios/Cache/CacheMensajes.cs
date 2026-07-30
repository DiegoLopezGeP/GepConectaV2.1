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
                    return contenedor.Mensajes.OrderByDescending(x => x.IdMensaje).ToList();
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
                        contenedor.Mensajes.OrderBy(x => x.Fecha);
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

        public async Task<bool> ActualizarEstadoPorWaidAsync(string wamid, string nuevoEstado)
        {
            if (string.IsNullOrEmpty(wamid)) return false;

            // 1. Definimos el peso de cada estado (Jerarquía)
            int ObtenerPesoEstado(string estado) => estado?.ToLower() switch
            {
                "sent" => 1,
                "delivered" => 2,
                "read" => 3,
                "failed" => 4,
                _ => 0
            };

            int nuevoPeso = ObtenerPesoEstado(nuevoEstado);

            // Intentamos buscar hasta 3 veces con breves pausas si el mensaje aún no se ha guardado en caché
            for (int intento = 0; intento < 3; intento++)
            {
                foreach (var contenedor in _cache.Values)
                {
                    lock (contenedor.CandadoInterno)
                    {
                        // Buscamos el mensaje por su WAMID de Meta
                        var mensaje = contenedor.Mensajes.FirstOrDefault(m => m.Waid == wamid);

                        if (mensaje != null)
                        {
                            int pesoActual = ObtenerPesoEstado(mensaje.EstadoLectura);

                            // SOLO actualizamos si el nuevo estado es superior al que ya tiene
                            if (nuevoPeso > pesoActual)
                            {
                                mensaje.EstadoLectura = nuevoEstado;
                                return true; // Actualizado con éxito
                            }

                            return false; // Ya tenía un estado igual o superior (evita degradar de "read" a "sent")
                        }
                    }
                }

                // Si no lo encontró en la primera iteración, esperamos 150ms a que el hilo de envío termine de guardar
                await Task.Delay(150);
            }

            Console.WriteLine($"[CACHE WARN]: No se encontró el mensaje con WAID '{wamid}' en caché para actualizar a '{nuevoEstado}'.");
            return false;
        }
    }
}