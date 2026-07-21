using WhatsappComercial.Enums;

namespace WhatsappComercial.Modelos.DTOs
{
    public class DatosTarjetaConversacionDTO
    {
        public int IdConversacion { get; set; }
        public string? NombreCliente { get; set; }
        public int IdTicket { get; set; }
        public DateTime? Fecha { get; set; }
        public EstadoConversacionEnum? EstadoConversacion { get; set; }
        public string nombreAsesor { get; set; }

        public bool esNuevaConversacion { get; set; }
    }
}
