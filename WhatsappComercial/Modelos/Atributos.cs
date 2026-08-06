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

        /// <summary>
        /// Indica si al perder el foco (Tab / OnBlur) en este campo se debe disparar la consulta
        /// para verificar si el cliente/contacto ya existe en la base de datos.
        /// </summary>
        public bool ValidarExistenciaOnBlur { get; set; } = false;

        // Constructor por defecto
        public FormCampoAttribute() { }

        // Constructor parametrizado actualizado
        public FormCampoAttribute(
            string label = "",
            bool requerido = false,
            int orden = 99,
            string mensajeRequerido = "",
            bool validarExistenciaOnBlur = false)
        {
            Label = label;
            Requerido = requerido;
            Orden = orden;
            ValidarExistenciaOnBlur = validarExistenciaOnBlur;
            MensajeRequerido = string.IsNullOrWhiteSpace(mensajeRequerido)
                ? $"El campo {label} es obligatorio."
                : mensajeRequerido;
        }
    }
}