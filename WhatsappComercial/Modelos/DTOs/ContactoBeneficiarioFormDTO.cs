namespace WhatsappComercial.Modelos.DTOs
{
    public class ContactoBeneficiarioFormDTO : ContactoBaseFormDTO
    {
        // IdCliente es oculto porque se asigna internamente desde la sesión/interfaz
        [FormCampo(Ocultar = true)]
        public int IdCliente { get; set; }

        [FormCampo(Label = "Nombre Completo", Requerido = true, Orden = 1)]
        public string NombreCompleto { get; set; } = string.Empty;

        [FormCampo(Label = "Teléfono", Requerido = true, Orden = 2)]
        public string Telefono { get; set; } = string.Empty;

        [FormCampo(Label = "Identificación", Requerido = false, Orden = 3)]
        public string? Identificacion { get; set; }

        [FormCampo(Label = "Edad", Requerido = true, Orden = 4)]
        public int Edad { get; set; }

        [FormCampo(Label = "Parentesco", Requerido = false, Orden = 5)]
        public int? IdParentesco { get; set; }

        // Campo especial (Pendiente de consulta externa)
        [FormCampo(Label = "Id Afiliaciones Beneficiario", Requerido = false, Orden = 6, Ocultar = true)]
        public int? IdBeneficiarioAfiliaciones { get; set; }
    }
}
