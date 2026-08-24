using WhatsappComercial.Interfaces.Cache;
using WhatsappComercial.Modelos;
using WhatsappComercial.Servicios.AccesoADatos;

namespace WhatsappComercial.Servicios.BackgroundServiceCache
{
    public class WorkerPersistenciaLogs : BackgroundService
    {
        private readonly IColaLogsEnvio _cola;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<WorkerPersistenciaLogs> _logger;

        public WorkerPersistenciaLogs(IColaLogsEnvio cola, IServiceScopeFactory scopeFactory, ILogger<WorkerPersistenciaLogs> logger)
        {
            _cola = cola;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (await _cola.Reader.WaitToReadAsync(stoppingToken))
                    {
                        while (_cola.Reader.TryRead(out var logItem))
                        {
                            await GuardarLogEnBDAsync(logItem);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[WORKER LOGS]: Ocurrió un error inesperado al procesar la cola de logs.");
                }
            }

            // Graceful shutdown
            while (_cola.Reader.TryRead(out var logRestante))
            {
                await GuardarLogEnBDAsync(logRestante);
            }
        }

        private async Task GuardarLogEnBDAsync(LogsEnvioMensajes log)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var servicioAccesoDatos = scope.ServiceProvider.GetRequiredService<ServicioAccesoDatos>();

                // Guarda el registro en la tabla LogsEnvioMensajes usando tu método de acceso a datos
                servicioAccesoDatos.GrabarRegistroDevuelveConsecutivo(log, "LogsEnvioMensajes");

                _logger.LogInformation($"[PERSISTENCIA LOG]: Se registró el log de error para el mensaje WAID: '{log.IdMensajeWhatsApp}'");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[WORKER LOGS]: Error guardando el log de error para el mensaje {log.IdMensajeWhatsApp}");
            }
        }
    }
}
