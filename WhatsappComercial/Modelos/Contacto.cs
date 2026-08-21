using WhatsappComercial.Enums;
using WhatsappComercial.Modelos;

namespace WhatsappComercial.Modelos
{
    public class Contacto
    {
        public int IdContacto { get; set; }
        public string? Prefijo { get; set; }
        public string NombreContacto { get; set; }

        public string CelularContacto { get; set; }
        public string?  NroIdentificacion { get; set; }
        public string?  CorreoElectronico { get; set; }
        public GeneroEnum Genero { get; set; }
        public string? NombreInstitucion { get; set; }
        public int? IdClienteBeneficiario { get; set; }
    }
}