using System.Data;
using System.Net.WebSockets;
using System.Security.Principal;
using System.Text;
using WhatsappComercial.Interfaces.ProcesarMensajeEntrante;
using WhatsappComercial.Servicios.AccesoADatos;
using WhatsappComercial.Servicios.ConfiguracionEstatica;

namespace WhatsappComercial.Servicios.WebSocketService
{
    public class WebSocketServicio : BackgroundService
    {

        private string _webSocketUri;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ConfiguracionEstaticaApp _configuracionEstatica = new ConfiguracionEstaticaApp();
        private readonly IHostApplicationLifetime _applicationLifetime;
        private static DataTable? configuracion;

        public WebSocketServicio(IServiceScopeFactory serviceScope, IHttpContextAccessor httpContextAccessor, IHostApplicationLifetime applicationLifetime)
        {

            _scopeFactory = serviceScope;
            _httpContextAccessor = httpContextAccessor;
            _applicationLifetime = applicationLifetime;
            _applicationLifetime.ApplicationStarted.Register(OnInit);
        }

        public async void OnInit()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _servicioAccesoDatos = scope.ServiceProvider.GetRequiredService<ServicioAccesoDatos>();
                configuracion = _servicioAccesoDatos.TraerTablaNombre("Configuraciones");
                _configuracionEstatica.EstablecerConfiguracion(configuracion);
                _webSocketUri = _configuracionEstatica.TraerConfiguracionPorCondicion("UrlWebSocket");

                DataTable esquemaMensaje = _servicioAccesoDatos.EsquemaTabla("Mensajes");
                _configuracionEstatica.EstablecerEsquema(esquemaMensaje);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var cliente = new ClientWebSocket();
                    await cliente.ConnectAsync(new Uri(_webSocketUri), stoppingToken);

                    var buffer = new byte[4096];

                    string usuario = string.Empty;

                    var context = _httpContextAccessor.HttpContext;
                    if (context?.User.Identity.IsAuthenticated == true)
                    {
                        usuario = context.User.Identity?.Name ?? "Desconocido";
                    }
                    else
                    {
                        usuario = WindowsIdentity.GetCurrent()?.Name;
                    }

                    while (cliente.State == WebSocketState.Open && !stoppingToken.IsCancellationRequested)
                    {
                        var result = await cliente.ReceiveAsync(new ArraySegment<byte>(buffer), stoppingToken);
                        var mensaje = Encoding.UTF8.GetString(buffer, 0, result.Count);

                        if (!string.IsNullOrWhiteSpace(mensaje) && mensaje != "Mensaje recibido")
                        {
                            using (var scope = _scopeFactory.CreateScope())
                            {
                                var procesarMensajeService = scope.ServiceProvider.GetRequiredService<IProcesarMensajeEntrante>();
                                await procesarMensajeService.ProcesarMensajeEntrante(mensaje, usuario);
                            }
                        }

                        await cliente.SendAsync(Encoding.UTF8.GetBytes("Received"), WebSocketMessageType.Text, true, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    await Task.Delay(2000, stoppingToken);
                }
                
            }
        }
    }
}
