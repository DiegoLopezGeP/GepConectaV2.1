using System.Collections.Concurrent;
using WhatsappComercial.Components.ComponentesHijo.Conversaciones;
using WhatsappComercial.Interfaces.Cache;
using WhatsappComercial.Modelos.DTOs;
using static WhatsappComercial.Components.ComponentesHijo.Conversaciones.VisorMensajes;

namespace WhatsappComercial.Servicios.Cache
{
    public class CacheMensajes : ICacheMensajes
    {
        private readonly int _capacidadMaxima;
        private readonly object _bloqueoOrden = new();

        private readonly ConcurrentDictionary<int, List<MensajeDTO>> _mensajesPorConversacion = new();

        // Lista enlazada para llevar el orden de "más recientemente usado"
        private readonly LinkedList<int> _ordenUso = new();
        private readonly Dictionary<int, LinkedListNode<int>> _nodosPorConversacion = new();

        public CacheMensajes(int capacidadMaxima = 20)
        {
            _capacidadMaxima = capacidadMaxima;
        }
        public void AgregarMensaje(int idConversacion, MensajeDTO mensaje)
        {
            if (_mensajesPorConversacion.TryGetValue(idConversacion, out var mensajes))
            {
                mensajes.Add(mensaje);
                MarcarComoUsadaRecientemente(idConversacion);
            }
            // Si no está en cache, no pasa nada: cuando el usuario la abra,
            // se hará la carga completa desde BD con el mensaje ya incluido.
        }

        public void AgregarMensajesAntiguos(int idConversacion, List<MensajeDTO> mensajesAntiguos)
        {
            if (_mensajesPorConversacion.TryGetValue(idConversacion, out var mensajes))
            {
                mensajes.InsertRange(0, mensajesAntiguos);
            }
        }

        public void GuardarEnCache(int idConversacion, List<MensajeDTO> mensajes)
        {
            _mensajesPorConversacion[idConversacion] = mensajes;
            MarcarComoUsadaRecientemente(idConversacion);
            AplicarLimiteCapacidad();
        }

        public List<MensajeDTO>? ObtenerDeCache(int idConversacion)
        {
            if (_mensajesPorConversacion.TryGetValue(idConversacion, out var mensajes))
            {
                MarcarComoUsadaRecientemente(idConversacion);
                return mensajes;
            }

            return null;
        }

        public void RemoverDeCache(int idConversacion)
        {
            _mensajesPorConversacion.TryRemove(idConversacion, out _);

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
                while (_ordenUso.Count > _capacidadMaxima)
                {
                    var idMenosReciente = _ordenUso.Last!.Value;
                    _ordenUso.RemoveLast();
                    _nodosPorConversacion.Remove(idMenosReciente);
                    _mensajesPorConversacion.TryRemove(idMenosReciente, out _);
                }
            }
        }
    }
}
