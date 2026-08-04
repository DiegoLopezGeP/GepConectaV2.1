namespace WhatsappComercial.Modelos
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FormCampoAttribute : Attribute
    {
        public string Label { get; set; } = string.Empty;
        public bool Requerido { get; set; } = false;
        public string MensajeRequerido { get; set; } = string.Empty;
        public int Orden { get; set; } = 99;
        public bool Ocultar { get; set; } = false;

        public FormCampoAttribute(string label = "", bool requerido = false, int orden = 99, string mensajeRequerido = "")
        {
            Label = label;
            Requerido = requerido;
            Orden = orden;
            MensajeRequerido = string.IsNullOrWhiteSpace(mensajeRequerido)
                ? $"El campo {label} es obligatorio."
                : mensajeRequerido;
        }
    }
}
