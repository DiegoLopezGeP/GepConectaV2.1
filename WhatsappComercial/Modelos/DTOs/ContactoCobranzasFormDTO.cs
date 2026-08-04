namespace WhatsappComercial.Modelos.DTOs
{
    public class ContactoCobranzasFormDTO : ContactoBaseFormDTO
    {
        [FormCampo(Label = "Nombre Completo", Requerido = true, Orden = 1)]
        public string Nombre { get; set; } = string.Empty;

        [FormCampo(Label = "Cédula / Identificación", Requerido = true, Orden = 2)]
        public string Ceda { get; set; } = string.Empty;

        [FormCampo(Label = "Teléfonos", Requerido = true, Orden = 3)]
        public string Telefonos { get; set; } = string.Empty;

        [FormCampo(Label = "Valor Mora", Requerido = true, Orden = 4)]
        public string ValorMora { get; set; } = string.Empty;

        [FormCampo(Label = "Valor con Descuento", Requerido = false, Orden = 5)]
        public string? ValorConDescuento { get; set; }
    }
}
