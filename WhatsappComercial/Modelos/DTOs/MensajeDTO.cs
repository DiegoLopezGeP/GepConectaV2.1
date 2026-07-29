namespace WhatsappComercial.Modelos.DTOs
{
    public class MensajeDTO
    {
        public int IdMensaje { get; set; }
        public string? Waid { get; set; }
        public string Texto { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public bool EsEntrante { get; set; }
        // Propiedades del Adjunto
        public string? NombreArchivo { get; set; }
        public string? TipoArchivo { get; set; }   // p. ej. "video/mp4" o "application/pdf" (MIME Type)
        public long TamanoArchivo { get; set; }    // Tamaño en bytes
        public string? HashSha256 { get; set; }     // Firma SHA256 proveniente de WhatsApp
        public string? UrlAdjunto { get; set; }
    }
}

