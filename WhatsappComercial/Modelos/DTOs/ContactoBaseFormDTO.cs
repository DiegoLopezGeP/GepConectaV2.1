using WhatsappComercial.Enums;

namespace WhatsappComercial.Modelos.DTOs
{
    public class ContactoBaseFormDTO
    {
        [FormCampo(Label = "Género", Requerido = true, Orden = 100)]
        public GeneroEnum Genero { get; set; } = GeneroEnum.Masculino;
    }
}
