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

        private readonly ServicioAccesoDatos _servicioAccesoDatos;
        private string _webSocketUri;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ConfiguracionApp setGetConfiguracion = new ConfiguracionApp();
        private static DataTable? configuracion;

        public WebSocketServicio(ServicioAccesoDatos servicioAccesoDatos, IServiceScopeFactory serviceScope, IHttpContextAccessor httpContextAccessor)
        {
            _servicioAccesoDatos = servicioAccesoDatos;
            _scopeFactory = serviceScope;
            _httpContextAccessor = httpContextAccessor;
        }

        public async void OnInit()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                configuracion = _servicioAccesoDatos.TraerTablaNombre("Configuracion");
                setGetConfiguracion.EstablecerConfiguracion(configuracion);
                _webSocketUri = setGetConfiguracion.TraerConfiguracionPorCondicion("UrlWebSocket");

                DataTable esquemaMensaje = _servicioAccesoDatos.EsquemaTabla("Mensajes");
                setGetConfiguracion.EstablecerEsquema(esquemaMensaje);
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
