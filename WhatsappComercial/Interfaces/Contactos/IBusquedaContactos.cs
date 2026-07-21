using WhatsappComercial.Modelos;

namespace WhatsappComercial.Interfaces.Contactos
{
    public interface IBusquedaContactos
    {
        Task<List<Contacto>> ObtenerContactoAsync(string filtroBusqueda);
        Task<List<Contacto>> ObtenerClienteTitularAsync(string filtroBusqueda);
        Task<List<Contacto>> ObtenerBeneficiarioAsync(string filtroBusqueda);
        Task<List<Contacto>> ObtenerClienteCobranzasAsync(string filtroBusqueda);
        Task<List<Contacto>> ObtenerClientePagaduriasAsync(string filtroBusqueda);
    }
}
