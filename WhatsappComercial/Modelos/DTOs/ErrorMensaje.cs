namespace WhatsappComercial.Modelos.DTOs
{
    public class ErrorMensaje
    {
        public int CodigoError { get; set; }
        public string TituloError { get; set; }
        public string DetalleError { get; set; }
        public string ExplicacionError { get; set; }
        public string PosibleSolucion { get; set; }
    }
}
