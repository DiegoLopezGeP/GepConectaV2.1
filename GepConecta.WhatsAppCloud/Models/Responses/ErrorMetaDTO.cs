using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace GepConecta.WhatsAppCloud.Models.Responses
{
    public class ErrorMetaDTO
    {
        [JsonProperty("message")]
        public string Mensaje { get; set; } = string.Empty;

        [JsonProperty("type")]
        public string Tipo { get; set; } = string.Empty;

        [JsonProperty("code")]
        public int Codigo { get; set; }
    }
}
