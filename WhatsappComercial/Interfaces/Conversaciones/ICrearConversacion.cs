using WhatsappComercial.Modelos;

namespace WhatsappComercial.Interfaces.Conversaciones
{
    public interface ICrearConversacion
    {
        Task CrearNuevaConversaciones(Convesacion conversacion);
    }
}
