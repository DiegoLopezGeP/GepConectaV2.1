namespace WhatsappComercial.Modelos
{
    public class Mensajes
    {
        public int IdMensaje { get; set; }
        public string? IdMensajeWhatsApp { get; set; }
        public string? ContenidoMensaje { get; set; }
        public DateTime? FechaEnvioMensaje { get; set; }
        public string? NombreUsuarioMensaje { get; set; }
        public string? TipoMensaje { get; set; }
        public int IdConversacion { get; set; }
        public bool MensajeEntranteMensaje { get; set; }
        public bool EsBot { get; set; }
        public string? MimeTypeMensaje { get; set; }
        public string? Sha256Mensaje { get; set; }
        public string?    NumTelefonoCliente { get; set; }
        public bool Leido { get; set; }
        public string? EstadoEnvio { get; set; }
        public string? Caption { get; set; }

    }
}
