using WhatsappComercial.Enums;
using WhatsappComercial.Interfaces.Contactos;

namespace WhatsappComercial.Servicios.Contactos
{
    public class HelperContacto : IHelperContacto
    {
        public string ObtenerNombreTabla(TipoContactoEnum tipoContacto)
        {
            return tipoContacto switch
            {
                TipoContactoEnum.Titular => "Clientes",
                TipoContactoEnum.Beneficiario => "Beneficiarios",
                TipoContactoEnum.Cobranzas => "BaseCobranzas",
                TipoContactoEnum.Pagadurias => "BaseTelemercadeo",
                TipoContactoEnum.Relacionista => "Contactos",
                _ => "Contacto"
            };
        }
    }
}
