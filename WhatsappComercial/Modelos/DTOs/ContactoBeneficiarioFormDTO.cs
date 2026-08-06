namespace WhatsappComercial.Modelos.DTOs
{
    public class ContactoBeneficiarioFormDTO : ContactoBaseFormDTO
    {
        // IdCliente es oculto porque se asigna internamente desde la sesión/interfaz
        [FormCampo(Ocultar = true)]
        public int IdCliente { get; set; }

        [FormCampo(Label = "Número Identificación Titular", Requerido = true, Orden = 1, ValidarExistenciaOnBlur = true)]
        public string NumeroIdentificacionTitular { get; set; } = string.Empty;

        [FormCampo(Label = "Nombre Completo", Requerido = true, Orden = 2)]
        public string NombreCompleto { get; set; } = string.Empty;

        [FormCampo(Label = "Teléfono", Requerido = true, Orden = 3)]
        public string Telefono { get; set; } = string.Empty;

        [FormCampo(Label = "Identificación", Requerido = false, Orden = 4)]
        public string? NumeroIdentificacion { get; set; }

        [FormCampo(Label = "Edad", Requerido = false, Orden = 5, Ocultar = true)]
        public int Edad { get; set; }

        [FormCampo(Label = "Parentesco", Requerido = false, Orden = 6, Ocultar = true)]
        public int? IdParentesco { get; set; }

        // Campo especial (Pendiente de consulta externa)
        [FormCampo(Label = "Id Afiliaciones Beneficiario", Requerido = false, Orden = 7, Ocultar = true)]
        public int? BeneficiarioAfiliacionesId { get; set; }
    }
}
