using System.Threading.Channels;
using WhatsappComercial.Modelos;

namespace WhatsappComercial.Interfaces.Cache
{
    public interface IColaLogsEnvio
    {
        void EncolarLog(LogsEnvioMensajes log);
        ChannelReader<LogsEnvioMensajes> Reader { get; }
    }
}
