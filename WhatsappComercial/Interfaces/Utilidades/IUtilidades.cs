using WhatsappComercial.Enums;
using WhatsappComercial.Modelos.DTOs;

namespace WhatsappComercial.Interfaces.Utilidades
{
    public interface IUtilidades
    {
        Task<DatosTarjetaConversacionDTO> ConstruirObjetoTarjetaConversacionAsync(int idConversacion, int idTicket, string nombreCliente, EstadoConversacionEnum estadoConversacion, DateTime fecha, string nombreUsuario, bool esNuevaConversacion);
    }
}
