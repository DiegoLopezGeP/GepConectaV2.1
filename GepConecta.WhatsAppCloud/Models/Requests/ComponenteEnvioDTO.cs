using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace GepConecta.WhatsAppCloud.Models.Requests
{
    public class ComponenteEnvioDTO
    {
        [JsonProperty("type")]
        public string Tipo { get; set; } = string.Empty; // "header", "body", "button"

        [JsonProperty("sub_type")]
        public string? SubTipo { get; set; } // Requerido solo cuando Tipo es "button": "url", "quick_reply"

        [JsonProperty("index")]
        public string? Indice { get; set; } // Requerido solo cuando Tipo es "button": "0", "1", "2"...

        [JsonProperty("parameters")]
        public List<ParametroEnvioDTO> Parametros { get; set; } = new();
    }
}
