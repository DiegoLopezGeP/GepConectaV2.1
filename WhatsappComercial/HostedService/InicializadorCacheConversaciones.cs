using WhatsappComercial.Interfaces.Cache;
using WhatsappComercial.Interfaces.Conversaciones;

namespace WhatsappComercial.HostedService
{
    public class InicializadorCacheConversaciones : IHostedService
    {
        private readonly IServiceProvider _proveedorServicios;

        public InicializadorCacheConversaciones(IServiceProvider proveedorServicios)
        {
            _proveedorServicios = proveedorServicios;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _proveedorServicios.CreateScope();
            var repositorio = scope.ServiceProvider.GetRequiredService<IObtenerConversaciones>();
            var cache = scope.ServiceProvider.GetRequiredService<ICacheConversaciones>();

            var activas = await repositorio.ObtenerConversacionesActivas();
            cache.CargarInicial(activas);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
