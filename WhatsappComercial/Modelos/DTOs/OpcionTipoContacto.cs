using WhatsappComercial.Enums;

namespace WhatsappComercial.Modelos.DTOs
{
    public class OpcionTipoContacto
    {
        public TipoContactoEnum Valor { get; set; }
        public string Texto { get; set; } = string.Empty;
    }
}
