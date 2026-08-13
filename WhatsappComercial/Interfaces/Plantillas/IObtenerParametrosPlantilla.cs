using System.Globalization;
using WhatsappComercial.Modelos.DTOs;

namespace WhatsappComercial.Interfaces.Plantillas
{
    public interface IObtenerParametrosPlantilla
    {
        Task<object> ObtenerParametrosPlantilla(string idPlantilla, DatosTarjetaConversacionDTO DatosTarjetaConversacionDTO);
    }
}
