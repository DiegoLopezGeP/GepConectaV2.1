using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace GepConecta.WhatsAppCloud.Models.Requests
{
    public class ParametroEnvioDTO
    {
        [JsonProperty("type")]
        public string Tipo { get; set; } = string.Empty; // "text", "image", "document", "payload"

        [JsonProperty("text")]
        public string? Texto { get; set; }

        [JsonProperty("image")]
        public RecursoMultimediaDTO? Imagen { get; set; }

        [JsonProperty("document")]
        public RecursoMultimediaDTO? Documento { get; set; }

        [JsonProperty("payload")]
        public string? Payload { get; set; } // Para botones Quick Reply
    }
}
