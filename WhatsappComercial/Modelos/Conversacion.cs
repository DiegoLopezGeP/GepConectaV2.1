namespace WhatsappComercial.Modelos
{
    public class Conversacion
    {
        public int IdConversacion { get; set; }
        public DateTime? FechaFinConversacion { get; set; }
        public int? IdCliente { get; set; }
        public int? IdBeneficiario { get; set; }
        public int? IdTicket { get; set; }
        public int IdWhatsappConversacion { get; set; }
        public DateTime FechaInicioConversacion { get; set; }
        public string? EstadoConversacion { get; set; }
        public bool  ActivaConAsesor { get; set; }
        public string? UltimaPlantilla { get; set; }
        public DateTime? FechaEnvioUltimaPlantilla { get; set; }
        public bool EsPrimeraInteraccion { get; set; }
        public int? IdClienteTemp { get; set; }
        public int IntentosInactividad { get; set; }
        public string? NumeroTelefonoConversacion { get; set; }
        public DateTime FechaUltimoMensaje { get; set; }
        public int IdTipificacion { get; set; }
        public int? IdBaseTelemercadeo { get; set; }
        public int? IdContacto { get; set; }

    }
}
