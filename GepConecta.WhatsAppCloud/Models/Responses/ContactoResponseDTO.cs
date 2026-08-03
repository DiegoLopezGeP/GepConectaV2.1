using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace GepConecta.WhatsAppCloud.Models.Responses
{
    public class ContactoResponseDTO
    {
        [JsonProperty("input")]
        public string Input { get; set; } = string.Empty;

        [JsonProperty("wa_id")]
        public string WaId { get; set; } = string.Empty;
    }
}
