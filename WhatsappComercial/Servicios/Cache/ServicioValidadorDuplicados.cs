using Microsoft.Extensions.Caching.Memory;
using WhatsappComercial.Interfaces.Cache;

namespace WhatsappComercial.Servicios.Cache
{
    public class ServicioValidadorDuplicados : IServicioValidadorDuplicados
    {
        private readonly IMemoryCache _cache;

        // Tiempo de vida del ID en la memoria antes de ser eliminado automáticamente
        private readonly TimeSpan _tiempoExpiracion = TimeSpan.FromMinutes(30);
        public ServicioValidadorDuplicados(IMemoryCache cache)
        {
            _cache = cache;
        }

        public bool EsMensajeDuplicado(string wamid)
        {
            if (string.IsNullOrWhiteSpace(wamid)) return false;

            string llaveCache = $"wamid_{wamid}";
            return _cache.TryGetValue(llaveCache, out _);
        }

        public void RegistrarMensajeProcesado(string wamid)
        {
            if (string.IsNullOrWhiteSpace(wamid)) return;

            string llaveCache = $"wamid_{wamid}";

            var opcionesCache = new MemoryCacheEntryOptions()
                // Vaciado automático: pasados 30 min, .NET destruye esta entrada de la RAM
                .SetAbsoluteExpiration(_tiempoExpiracion)
                // Prioridad baja para que si el servidor se queda sin RAM, la purga primero
                .SetPriority(CacheItemPriority.Low);

            _cache.Set(llaveCache, true, opcionesCache);
        }
    }
}

