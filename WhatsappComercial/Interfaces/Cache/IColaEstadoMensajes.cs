using System.Threading.Channels;
using WhatsappComercial.Modelos.DTOs;

namespace WhatsappComercial.Interfaces.Cache
{
    public interface IColaEstadoMensajes
    {
        void EncolarEstado(EstadoMensajeItem item);
        ChannelReader<EstadoMensajeItem> Reader { get; }
    }
}
