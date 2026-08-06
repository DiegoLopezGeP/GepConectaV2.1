namespace WhatsappComercial.Modelos.DTOs
{
    public class BaseCobranzas
    {
        public int IdRegistro { get; set; }
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string ValorMora { get; set; }
        public string? ValorConDescuento { get; set; }
        public int Genero { get; set; }
    }
}
