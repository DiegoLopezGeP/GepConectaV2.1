using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GepConecta.WhatsAppCloud.Models.Responses
{
    public class PeticionBaseWhatsApp
    {
        [JsonPropertyName("messaging_product")]
        public string messaging_product { get; set; } = "whatsapp";

        [JsonPropertyName("recipient_type")]
        public string recipient_type { get; set; } = "individual";

        [JsonPropertyName("to")]
        public required string to { get; set; }

        [JsonPropertyName("type")]
        public string type { get; set; } = string.Empty; // <-- Se le quita 'required'
    }
}
