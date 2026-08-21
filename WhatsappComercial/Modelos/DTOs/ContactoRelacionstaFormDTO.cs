using System.Reflection.Emit;

namespace WhatsappComercial.Modelos.DTOs
{
    public class ContactoRelacionstaFormDTO : ContactoBaseFormDTO
    {
        [FormCampo(Label = "Prefijo", Requerido = true, Orden = 1)]
        public string Prefijo { get; set; } = string.Empty;

        [FormCampo(Label = "Nombre Contacto", Requerido = true, Orden = 2)]
        public string NombreContacto { get; set; } = string.Empty;

        [FormCampo(Label = "Celular Contacto", Requerido = true, Orden = 3)]
        public string CelularContacto { get; set; } = string.Empty;

        [FormCampo(Label = "Nro. Identificacion ", Requerido = true, Orden = 4)]
        public string NroIdentificacion { get; set; } = string.Empty;

        [FormCampo(Label = "Nombre Institución ", Requerido = true, Orden = 5)]
        public string NombreInstitucion { get; set; } = string.Empty;
    }
}
