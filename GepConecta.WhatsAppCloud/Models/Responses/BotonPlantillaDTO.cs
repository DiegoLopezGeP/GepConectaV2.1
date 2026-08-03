using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace GepConecta.WhatsAppCloud.Models.Responses
{
    public class BotonPlantillaDTO
    {
        [JsonProperty("type")]
        public string Tipo { get; set; } = string.Empty; // QUICK_REPLY, PHONE_NUMBER, URL

        [JsonProperty("text")]
        public string Texto { get; set; } = string.Empty;

        [JsonProperty("url")]
        public string? Url { get; set; }

        [JsonProperty("phone_number")]
        public string? NumeroTelefono { get; set; }
    }
}
