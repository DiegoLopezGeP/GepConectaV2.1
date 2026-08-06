using WhatsappComercial.Modelos;

namespace WhatsappComercial.Interfaces.Contactos
{
    public interface IBusquedaContactoAfiliaciones
    {
        Task<List<Contacto>> BuscarTitularAfiliaciones(string filtro);
        Task<List<Contacto>> BuscarBeneficiarioAfiliaciones(string filtro);
    }
}
