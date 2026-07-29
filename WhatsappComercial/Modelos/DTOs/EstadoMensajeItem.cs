namespace WhatsappComercial.Modelos.DTOs
{
    public class EstadoMensajeItem
    {
        public int IdConversacion { get; set; }
        public string IdMensajeWhatsApp { get; set; } = string.Empty;
        public string EstadoEnvio { get; set; } = string.Empty; // Ej: "sent", "delivered", "read"

    }
}
