using System.Collections.Concurrent;
using System.Data;
using Serilog;
using Serilog.Core;
using WhatsappComercial.Interfaces.Cache;
using WhatsappComercial.Modelos.DTOs;
using WhatsappComercial.Servicios.AccesoADatos;

namespace WhatsappComercial.Servicios.Cache
{
    public class CacheConversacionesService : ICacheConversaciones
    {
        public event Func<DatosTarjetaConversacionDTO, Task>? ConversacionActualizada;
        public event Func<int, Task>? ConversacionFinalizada;

        // Índice principal: conversación por Id
        private readonly ConcurrentDictionary<int, DatosTarjetaConversacionDTO> _conversaciones = new();
        private readonly ConcurrentDictionary<string, int> _indicePorTelefono = new();

        public Task ActualizarUltimoMensajeAsync(int conversacionId, string preview, DateTime fecha, bool incrementarNoLeidos)
        {
            throw new NotImplementedException();
        }

        public void CargarInicial(IEnumerable<DatosTarjetaConversacionDTO> conversaciones)
        {
            foreach (var conversacion in conversaciones)
            {
                _conversaciones[conversacion.IdConversacion] = conversacion;
                if (!string.IsNullOrWhiteSpace(conversacion.NumeroCelular))
                {
                    _indicePorTelefono[conversacion.NumeroCelular] = conversacion.IdConversacion;
                }
            }

            Console.WriteLine($"Cache de conversaciones inicializado con {_conversaciones.Count} conversaciones activas");
        }

        public async Task<DatosTarjetaConversacionDTO> CrearOActualizarAsync(DatosTarjetaConversacionDTO conversacion)
        {
            _conversaciones.AddOrUpdate(conversacion.IdConversacion, conversacion, (_, existente) => conversacion);

            if (!string.IsNullOrWhiteSpace(conversacion.NumeroCelular))
            {
                _indicePorTelefono[conversacion.NumeroCelular] = conversacion.IdConversacion;
            }

            if (ConversacionActualizada != null)
            {
                await ConversacionActualizada.Invoke(conversacion);
            }

            return conversacion;
        }

        public async Task FinalizarAsync(int conversacionId)
        {
            // 1 Remover del caché activo en memoria (ConcurrentDictionary / Dictionary)
            // Asumiendo que tu diccionario en el servicio se llama _conversacionesActivas:
            bool removido = _conversaciones.TryRemove(conversacionId, out var conversacionRemovida);

            // 2. Notificar a todos los componentes Blazor suscritos que el chat finalizó
            if (ConversacionFinalizada != null)
            {
                await ConversacionFinalizada.Invoke(conversacionId);
            }
        }

        public IEnumerable<DatosTarjetaConversacionDTO> ObtenerActivasPorArea(int areaId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<DatosTarjetaConversacionDTO>> ObtenerActivasPorUsuario(string nombreUsuario)
        {
            List<DatosTarjetaConversacionDTO> conversaciones = _conversaciones
                .Where(c => c.Value.nombreAsesor == nombreUsuario)
                .Select(c => c.Value)
                .OrderByDescending(c => c.Fecha)
                .ToList();

            return conversaciones;
        }

        public async Task<DatosTarjetaConversacionDTO?> ObtenerConversacionPorNumeroTelefono(string numeroTelefono)
        {
            if (string.IsNullOrWhiteSpace(numeroTelefono)) return null;

            // Búsqueda directa O(1) por número de teléfono
            if (_indicePorTelefono.TryGetValue(numeroTelefono, out int conversacionId))
            {
                if (_conversaciones.TryGetValue(conversacionId, out var conversacion))
                {
                    return conversacion;
                }
            }

            return null;
        }
        public Task ReasignarAsync(int conversacionId, int nuevoUsuarioId)
        {
            throw new NotImplementedException();
        }
    }
}
