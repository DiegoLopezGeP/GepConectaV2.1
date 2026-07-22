using WhatsappComercial.Enums;

namespace WhatsappComercial.Modelos.DTOs
{
    public class DatosContactoConversacionDTO
    {
        public int IdTicket { get; set; }
        public string? NombreCliente  { get; set; }
        public string? Celular { get; set; }
        public string? Identificacion { get; set; }
        public  EstadoConversacionEnum EstadoConversacion { get; set; }
        public DateTime? FechaInicioConversacion { get; set; }
        public DateTime? FechaFinConversacion { get; set; }
    }
}
