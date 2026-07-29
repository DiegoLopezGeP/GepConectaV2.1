using System.Threading.Channels;
using WhatsappComercial.Interfaces.Cache;
using WhatsappComercial.Modelos.DTOs;

namespace WhatsappComercial.Servicios.Cache
{
    public class ColaEstadoMensajesService : IColaEstadoMensajes
    {
        // Unbounded channel optimizado para 1 solo consumidor (el Worker)
        private readonly Channel<EstadoMensajeItem> _channel = Channel.CreateUnbounded<EstadoMensajeItem>(
            new UnboundedChannelOptions
            {
                SingleReader = true
            });

        public void EncolarEstado(EstadoMensajeItem item)
        {
            _channel.Writer.TryWrite(item);
        }

        public ChannelReader<EstadoMensajeItem> Reader => _channel.Reader;
    }
}
