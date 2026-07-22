namespace WhatsappComercial.Modelos.DTOs
{
    public class MensajeDTO
    {
        public int IdMensaje { get; set; }
        public string Texto { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public bool EsEntrante { get; set; }
    }
}

