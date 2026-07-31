using WhatsappComercial.Enums;

namespace WhatsappComercial.Interfaces.Conversaciones
{
    public interface IFinalizarConversacion
    {
        Task FinalizarConversacionManual(int idConversacion, EstadoConversacionEnum estadoFinalizado);
    }
}
