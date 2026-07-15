using WhatsappComercial.Modelos;

namespace WhatsappComercial.Interfaces.Contactos
{
    public interface IBusquedaContactos
    {
        Task<List<Contacto>> ObtenerContactoAsync(string filtroBusqueda);
    }
}
