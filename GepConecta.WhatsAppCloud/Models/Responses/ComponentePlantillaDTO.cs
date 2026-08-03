using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using WhatsappComercial.Modelos.DTOs;

namespace GepConecta.WhatsAppCloud.Models.Responses
{
    public class ComponentePlantillaDTO
    {
        [JsonProperty("type")]
        public string Tipo { get; set; } = string.Empty; // HEADER, BODY, FOOTER, BUTTONS

        [JsonProperty("text")]
        public string Texto { get; set; } = string.Empty;

        [JsonProperty("format")]
        public string Format { get; set; } = string.Empty; // TEXT, IMAGE, DOCUMENT, etc.

        [JsonProperty("example")]
        public EjemploComponenteDTO? Ejemplo { get; set; }

        [JsonProperty("buttons")]
        public List<BotonPlantillaDTO>? Botones { get; set; }
    }
}
