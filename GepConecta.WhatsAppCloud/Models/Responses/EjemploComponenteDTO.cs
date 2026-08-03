using Newtonsoft.Json;

namespace WhatsappComercial.Modelos.DTOs
{
    public class EjemploComponenteDTO
    {
        [JsonProperty("body_text")]
        public List<List<string>>? TextoCuerpoVariables { get; set; } // Representa p. ej: [["Nombre del Cliente", "Nombre del asesor"]]

        [JsonProperty("header_text")]
        public List<string>? TextoEncabezadoVariables { get; set; }

        [JsonProperty("header_handle")]
        public List<string>? HeadersArchivos { get; set; }
    }
}
