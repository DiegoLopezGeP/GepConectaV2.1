using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace GepConecta.WhatsAppCloud.Models.Responses
{
    public class PlantillaMetaResponseDTO
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string Nombre { get; set; } = string.Empty;

        [JsonProperty("language")]
        public string Idioma { get; set; } = string.Empty;

        [JsonProperty("status")]
        public string Estado { get; set; } = string.Empty;

        [JsonProperty("category")]
        public string Categoria { get; set; } = string.Empty;

        [JsonProperty("sub_category")]
        public string? SubCategoria { get; set; }

        [JsonProperty("parameter_format")]
        public string? FormatoParametro { get; set; }

        [JsonProperty("components")]
        public List<ComponentePlantillaDTO> Componentes { get; set; } = new();
    }
   

}
