using System.Reflection.Emit;

namespace WhatsappComercial.Modelos.DTOs
{
    public class ContactoRelacionstaFormDTO : ContactoBaseFormDTO
    {
        [FormCampo(Label = "Nombre Contacto", Requerido = true, Orden = 1)]
        public string NombreContacto { get; set; } = string.Empty;

        [FormCampo(Label = "Celular Contacto", Requerido = true, Orden = 2)]
        public string CelularContacto { get; set; } = string.Empty;
    }
}
