namespace WhatsappComercial.Modelos
{
    public class Contacto
    {
        public int IdContacto { get; set; }
        public string NombreContacto { get; set; }
        public string CelularContacto { get; set; }
        public string?  Identificacion { get; set; }
        public string?  CorreoElectronico { get; set; }
        public int Genero { get; set; }

        public int? IdClienteBeneficiario { get; set; }
    }
}
