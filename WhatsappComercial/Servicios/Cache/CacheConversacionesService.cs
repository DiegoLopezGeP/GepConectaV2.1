using System.Collections.Concurrent;
using System.Data;
using Serilog;
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
        public Task ActualizarUltimoMensajeAsync(int conversacionId, string preview, DateTime fecha, bool incrementarNoLeidos)
        {
            throw new NotImplementedException();
        }

        public void CargarInicial(IEnumerable<DatosTarjetaConversacionDTO> conversaciones)
        {
            foreach (var conversacion in conversaciones)
            {
                _conversaciones[conversacion.IdConversacion] = conversacion;
            }

            Console.WriteLine($"Cache de conversaciones inicializado con {_conversaciones.Count} conversaciones activas");
        }

        public Task<DatosTarjetaConversacionDTO> CrearOActualizarAsync(DatosTarjetaConversacionDTO conversacion)
        {
            throw new NotImplementedException();
        }

        public Task FinalizarAsync(int conversacionId)
        {
            throw new NotImplementedException();
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
                .ToList();

            return conversaciones;
        }

        public DatosTarjetaConversacionDTO? ObtenerPorId(int conversacionId)
        {
            throw new NotImplementedException();
        }

        public Task ReasignarAsync(int conversacionId, int nuevoUsuarioId)
        {
            throw new NotImplementedException();
        }
    }
}
