namespace WhatsappComercial.Modelos
{
    public class LogsEnvioMensajes
    {

        public int IdLogMensaje { get; set; }
        public string IdMensajeWhatsApp { get; set; }
        public string EstadoMensaje { get; set; }
        public DateTime? FechaEstado { get; set; }
        public string NumeroReceptor { get; set; }
        public int CodigoError { get; set; }
        public string DetalleError { get; set; }
        public DateTime FechaRegistroLog { get; set; }
        public string UsuarioEnvioMensaje { get; set; }
        public int IdConversacion { get; set; }




    }

}
