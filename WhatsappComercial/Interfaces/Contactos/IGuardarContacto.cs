using WhatsappComercial.Modelos;

namespace WhatsappComercial.Interfaces.Contactos
{
    public interface IGuardarContacto
    {
        Task GuardarContactoAsync(Contacto contacto);
    }
}
