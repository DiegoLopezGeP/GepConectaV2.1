using System.Threading.Channels;
using WhatsappComercial.Interfaces.Cache;
using WhatsappComercial.Modelos;

namespace WhatsappComercial.Servicios.Cache
{
    public class ColaLogsEnvioService : IColaLogsEnvio
    {
        private readonly Channel<LogsEnvioMensajes> _channel = Channel.CreateUnbounded<LogsEnvioMensajes>(
            new UnboundedChannelOptions
            {
                SingleReader = true
            });

        public void EncolarLog(LogsEnvioMensajes log)
        {
            _channel.Writer.TryWrite(log);
        }

        public ChannelReader<LogsEnvioMensajes> Reader => _channel.Reader;
    }
}
