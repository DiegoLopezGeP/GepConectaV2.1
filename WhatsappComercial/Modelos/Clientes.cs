using WhatsappComercial.Enums;

namespace WhatsappComercial.Modelos
{
    public class Clientes
    {
        public int IdCliente { get; set; }
        public string NombreCompletoCliente { get; set; }
        public string NumCelularCliente { get; set; }
        public string NumIdentificacionCliente { get; set; }
        public string?  Profesion { get; set; }
        public string CorreoElectronico { get; set; }
        public int IdClienteAfiliaciones { get; set; }
        public GeneroEnum Genero { get; set; }
    }
}
