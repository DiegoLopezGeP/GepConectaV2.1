using WhatsappComercial.Modelos;

namespace WhatsappComercial.Interfaces.Conversaciones
{
    public interface ICrearConversacion
    {
        Task<int> CrearNuevaConversaciones(Conversacion conversacion);
    }
}
