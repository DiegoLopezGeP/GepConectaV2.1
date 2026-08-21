using System.Data;
using System.Net.WebSockets;
using System.Text;
using WhatsappComercial.Interfaces.ProcesarMensajeEntrante;
using WhatsappComercial.Servicios.AccesoADatos;
using WhatsappComercial.Servicios.ConfiguracionEstatica;

namespace WhatsappComercial.Servicios.WebSocketService
{
    public class WebSocketServicio : BackgroundService
    {
        private string? _webSocketUri;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ConfiguracionEstaticaApp _configuracionEstatica = new ConfiguracionEstaticaApp();
        private static DataTable? configuracion;

        public WebSocketServicio(IServiceScopeFactory serviceScope)
        {
            _scopeFactory = serviceScope;
        }

        private void CargarConfiguracion()
        {
            using var scope = _scopeFactory.CreateScope();
            var servicioAccesoDatos = scope.ServiceProvider.GetRequiredService<ServicioAccesoDatos>();

            configuracion = servicioAccesoDatos.TraerTablaNombre("Configuraciones");
            _configuracionEstatica.EstablecerConfiguracion(configuracion);
            //_webSocketUri = _configuracionEstatica.TraerConfiguracionPorCondicion("UrlWebSocket");
            _webSocketUri = _configuracionEstatica.TraerConfiguracionPorCondicion("UrlWebSocketWatCom");

            DataTable esquemaMensaje = servicioAccesoDatos.EsquemaTabla("Mensajes");
            _configuracionEstatica.EstablecerEsquema(esquemaMensaje);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // 1. Cargar configuración de forma sincrónica y segura ANTES de conectar
            try
            {
                CargarConfiguracion();
            }
            catch (Exception ex)
            {
                // Si falla la BD al iniciar, reintentamos o registramos el error
                await Task.Delay(3000, stoppingToken);
            }

            // 2. Definir un usuario del sistema para procesos en segundo plano
            string usuarioServicio = "Sistema_WebSocketService";

            // 3. Bucle de conexión
            while (!stoppingToken.IsCancellationRequested)
            {
                // Validar que tengamos una URI válida cargada
                if (string.IsNullOrWhiteSpace(_webSocketUri))
                {
                    try
                    {
                        CargarConfiguracion();
                    }
                    catch
                    {
                        await Task.Delay(5000, stoppingToken);
                        continue;
                    }
                }

                try
                {
                    using var cliente = new ClientWebSocket();
                    await cliente.ConnectAsync(new Uri(_webSocketUri!), stoppingToken);

                    var buffer = new byte[4096];

                    while (cliente.State == WebSocketState.Open && !stoppingToken.IsCancellationRequested)
                    {
                        var result = await cliente.ReceiveAsync(new ArraySegment<byte>(buffer), stoppingToken);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await cliente.CloseAsync(WebSocketCloseStatus.NormalClosure, "Cierre solicitado", stoppingToken);
                            break;
                        }

                        var mensaje = Encoding.UTF8.GetString(buffer, 0, result.Count);

                        if (!string.IsNullOrWhiteSpace(mensaje) && mensaje != "Mensaje recibido")
                        {
                            using (var scope = _scopeFactory.CreateScope())
                            {
                                var procesarMensajeService = scope.ServiceProvider.GetRequiredService<IProcesarMensajeEntrante>();
                                await procesarMensajeService.ProcesarMensajeEntrante(mensaje, usuarioServicio);
                            }
                        }

                        await cliente.SendAsync(Encoding.UTF8.GetBytes("Received"), WebSocketMessageType.Text, true, stoppingToken);
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // La aplicación se está apagando normalmente
                    break;
                }
                catch (Exception ex)
                {
                    // Esperar antes de intentar reconectar si el WebSocket externo cayó
                    await Task.Delay(3000, stoppingToken);
                }
            }
        }
    }
}