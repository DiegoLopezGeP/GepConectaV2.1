namespace WhatsappComercial.Modelos.DTOs
{
    public class CampoEsquemaDTO
    {
        public string NombreCampo { get; set; } = string.Empty;

        private string _etiqueta = string.Empty;
        public string Etiqueta
        {
            get => !string.IsNullOrWhiteSpace(_etiqueta)
                ? _etiqueta
                : System.Text.RegularExpressions.Regex.Replace(NombreCampo, "(\\B[A-Z])", " $1");
            set => _etiqueta = value;
        }

        public Type TipoDato { get; set; } = typeof(string);
        public bool EsObligatorio { get; set; }
        public string MensajeError { get; set; } = string.Empty;
        public int Orden { get; set; } = 99;
        public int? LongitudMaxima { get; set; }
        public bool EsClavePrimaria { get; set; }
        public bool EsAutoincremental { get; set; }
    }
}