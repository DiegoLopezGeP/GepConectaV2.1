using WhatsappComercial.Enums;

namespace WhatsappComercial.Modelos
{
    public class Beneficiario
    {
        public int IdBeneficiario { get; set; }
        public int? IdCliente { get; set; }
        public int? IdParentesco { get; set; }
        public string NombreCompleto { get; set; }
        public string Identificacion { get; set; }
        public string Telefono { get; set; }
        public GeneroEnum Genero { get; set; }
        public int? Edad { get; set; }
        public int IdBeneficiarioAfiliaciones { get; set; }
    }
}
