using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GepConecta.WhatsAppCloud.Models.Responses
{
    public class EnviarRespuestaWhatsApp
    {
        [JsonPropertyName("messaging_product")]
        public string MessagingProduct { get; set; } = string.Empty;

        [JsonPropertyName("contacts")]
        public List<WhatsAppContact>? Contacts { get; set; }

        [JsonPropertyName("messages")]
        public List<WhatsAppMessageId>? Messages { get; set; }
    }
    public class WhatsAppContact
    {
        [JsonPropertyName("input")]
        public string Input { get; set; } = string.Empty;

        [JsonPropertyName("wa_id")]
        public string WaId { get; set; } = string.Empty;
    }

    public class WhatsAppMessageId
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
    }

    // Respuesta al subir una nota de voz o archivo
    public class WhatsAppMediaUploadResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
    }
    
}
