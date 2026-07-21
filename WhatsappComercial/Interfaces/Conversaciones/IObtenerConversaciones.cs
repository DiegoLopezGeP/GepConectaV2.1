using WhatsappComercial.Modelos;
using WhatsappComercial.Modelos.DTOs;

namespace WhatsappComercial.Interfaces.Conversaciones
{
    public interface IObtenerConversaciones
    {
        Task<List<DatosTarjetaConversacionDTO>> ObtenerDatosConversacionContacto(GrupoUser informacionUsuarioAutenticado);
        Task<List<DatosTarjetaConversacionDTO>> ObtenerConversacionesActivas();
    }
}
