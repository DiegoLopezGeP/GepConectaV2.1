using WhatsappComercial.Modelos.DTOs;

namespace WhatsappComercial.Interfaces.Plantillas
{
    public interface IObtenerPlantillas
    {
        Task<List<PlantillaDTO>> ObtenerPlantillasActivas();
    }
}
