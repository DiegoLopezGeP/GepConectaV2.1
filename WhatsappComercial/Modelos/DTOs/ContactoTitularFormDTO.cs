namespace WhatsappComercial.Modelos.DTOs
{
    public class ContactoTitularFormDTO : ContactoBaseFormDTO
    {
        [FormCampo(Label = "Número de Identificación", Requerido = true, Orden = 1, ValidarExistenciaOnBlur = true)]
        public string NumIdentificacionCliente { get; set; } = string.Empty;

        [FormCampo(Label = "Nombre Completo", Requerido = true, Orden = 2)]
        public string NombreCompletoCliente { get; set; } = string.Empty;

        [FormCampo(Label = "Número de Celular", Requerido = true, Orden = 3)]
        public string NumCelularCliente { get; set; } = string.Empty;

        [FormCampo(Label = "Profesión", Requerido = false, Orden = 4)]
        public string? Profesion { get; set; }

        [FormCampo(Label = "Correo Electrónico", Requerido = true, Orden = 5)]
        public string CorreoElectronico { get; set; } = string.Empty;

        // Campo especial (Pendiente de validación externa, no requerido por ahora)
        [FormCampo(Label = "Id Afiliaciones", Requerido = false, Orden = 6, Ocultar = true)]
        public int? IdClienteAfiliaciones { get; set; }
    }
}
