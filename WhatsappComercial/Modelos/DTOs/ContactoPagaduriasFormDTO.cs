using WhatsappComercial.Enums;

namespace WhatsappComercial.Modelos.DTOs
{
    public class ContactoPagaduriasFormDTO : ContactoBaseFormDTO
    {
        [FormCampo(Label = "Pagaduría", Requerido = true, Orden = 1)]
        public PagaduriaEnum Pagaduria { get; set; } = PagaduriaEnum.Seleccione;

        [FormCampo(Label = "Nombre Completo", Requerido = true, Orden = 2)]
        public string Nombre { get; set; } = string.Empty;

        [FormCampo(Label = "Identificación", Requerido = true, Orden = 3)]
        public string Identificacion { get; set; } = string.Empty;

        [FormCampo(Label = "Celular", Requerido = true, Orden = 4)]
        public string Celular { get; set; } = string.Empty;

        [FormCampo(Label = "Base", Requerido = false, Orden = 5)]
        public string? Base { get; set; }

        [FormCampo(Label = "Tipo Pagaduría", Requerido = false, Orden = 6)]
        public string? TipoPagaduria { get; set; }
    }
}
