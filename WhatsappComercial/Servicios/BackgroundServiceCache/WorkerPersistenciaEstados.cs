using WhatsappComercial.Interfaces.Cache;
using WhatsappComercial.Modelos.DTOs;
using WhatsappComercial.Servicios.AccesoADatos;

namespace WhatsappComercial.Servicios.BackgroundServiceCache
{
    public class WorkerPersistenciaEstados : BackgroundService
    {
        private readonly IColaEstadoMensajes _cola;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<WorkerPersistenciaEstados> _logger;


        // Configuración del Batch
        private const int TAMANO_MAX_LOTE = 2;
        private const int TIEMPO_ESPERA_SEGUNDOS = 50;

        public WorkerPersistenciaEstados(IColaEstadoMensajes cola, IServiceScopeFactory scopeFactory, ILogger<WorkerPersistenciaEstados> logger)
        {
            _cola = cola;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var lote = new List<EstadoMensajeItem>();
            var ultimoProcesamiento = DateTime.UtcNow;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Espera asíncrona hasta que haya al menos un elemento o el token solicite cancelación
                    if (await _cola.Reader.WaitToReadAsync(stoppingToken))
                    {
                        while (_cola.Reader.TryRead(out var item))
                        {
                            lote.Add(item);

                            // Regla 1: Si alcanzamos el tamaño máximo del lote, procesamos de inmediato
                            if (lote.Count >= TAMANO_MAX_LOTE)
                            {
                                await ProcesarGuardadoEnBDAsync(lote);
                                lote.Clear();
                                ultimoProcesamiento = DateTime.UtcNow;
                            }
                        }
                    }

                    // Regla 2: Si hay elementos pendientes y pasaron más de N segundos, procesamos
                    if (lote.Any() && (DateTime.UtcNow - ultimoProcesamiento).TotalSeconds >= TIEMPO_ESPERA_SEGUNDOS)
                    {
                        await ProcesarGuardadoEnBDAsync(lote);
                        lote.Clear();
                        ultimoProcesamiento = DateTime.UtcNow;
                    }
                }
                catch (OperationCanceledException)
                {
                    // La aplicación inició el proceso de apagado; la captura se maneja abajo
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[WORKER ESTADOS]: Ocurrió un error inesperado procesando el lote de estados.");
                }
            }

            // =========================================================================
            // GRACEFUL SHUTDOWN: Se ejecuta si la app se apaga/reinicia
            // =========================================================================
            _logger.LogWarning("[WORKER ESTADOS]: Apagando servicio... Procesando elementos restantes de la cola.");

            while (_cola.Reader.TryRead(out var itemRestante))
            {
                lote.Add(itemRestante);
            }

            if (lote.Any())
            {
                await ProcesarGuardadoEnBDAsync(lote);
                _logger.LogInformation($"[WORKER ESTADOS]: Se guardaron {lote.Count} estados pendientes exitosamente durante el apagar.");
            }
        }

        private async Task ProcesarGuardadoEnBDAsync(List<EstadoMensajeItem> lote)
        {
            if (!lote.Any()) return;

            try
            {
                // Creamos un Scope para consumir servicios Scoped (como tu DbContext)
                using var scope = _scopeFactory.CreateScope();


                // AQUÍ INVOCAS TU SERVICIO / REPOSITORIO DE BASE DE DATOS
                // Ejemplo:
                var _servicioAccesoDatos = scope.ServiceProvider.GetRequiredService<ServicioAccesoDatos>();

                string[] parametrosCampos = ["EstadoEnvio"];

                foreach (var itemParaActualizar in lote)
                {
                    _servicioAccesoDatos.ActualizarCampo("Mensajes", "EstadoEnvio", $"'{itemParaActualizar.EstadoEnvio}'", $"IdConversacion = {itemParaActualizar.IdConversacion} and idMensajeWhatsApp = '{itemParaActualizar.IdMensajeWhatsApp}'" );
                }


                _logger.LogInformation($"[PERSISTENCIA BATCH]: Se actualizaron {lote.Count} estados en la base de datos.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[WORKER ESTADOS]: Fallo al persistir lote de {lote.Count} registros en la base de datos.");
            }
        }
    }
}
