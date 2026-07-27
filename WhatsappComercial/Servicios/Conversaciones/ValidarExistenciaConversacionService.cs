using WhatsappComercial.Interfaces.Cache;
using WhatsappComercial.Interfaces.Conversaciones;
using WhatsappComercial.Modelos;
using WhatsappComercial.Modelos.DTOs;
using WhatsappComercial.Servicios.AccesoADatos;
using WhatsappComercial.Servicios.Cache;

namespace WhatsappComercial.Servicios.Conversaciones
{
    public class ValidarExistenciaConversacionService : IValidarExistenciaConversacion
    {
        private readonly ICacheConversaciones _cacheConversacion;
        private readonly ServicioAccesoDatos _datos;

        public ValidarExistenciaConversacionService(ICacheConversaciones cacheConversacion, ServicioAccesoDatos datos)
        {
            _cacheConversacion = cacheConversacion;
            _datos = datos;
        }

        public async Task<int> ExisteConversacion(string numeroTelefonoConversacion)
        {
            if (string.IsNullOrWhiteSpace(numeroTelefonoConversacion))
                return 0;

            try
            {
                // Buscar directamente en la Caché en memoria
                DatosTarjetaConversacionDTO? conversacionCache = await _cacheConversacion.ObtenerConversacionPorNumeroTelefono(numeroTelefonoConversacion);

                if (conversacionCache != null)
                {
                    // Encontrada en RAM: Retornamos el Id de la conversación
                    return conversacionCache.IdConversacion;
                }

                // Si no existe en la caché, retornamos 0
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR EN VALIDAR EXISTENCIA CONVERSACIÓN]: {ex.Message}");
                return 0;
            }
        }
    }
}