using WhatsappComercial.Enums;

namespace WhatsappComercial.Interfaces.Contactos
{
    public interface IActualizarContacto
    {
        Task ActualizarContactoDinamicoAsync(TipoContactoEnum tipoContacto, Dictionary<string, object?> datos);
    }
}
