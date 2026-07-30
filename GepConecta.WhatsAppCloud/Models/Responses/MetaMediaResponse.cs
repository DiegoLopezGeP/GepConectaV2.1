using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GepConecta.WhatsAppCloud.Models.Responses
{
    // DTO interno para deserializar la respuesta del Paso 1
    internal class MetaMediaResponse
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("mime_type")]
        public string MimeType { get; set; } = string.Empty;

        [JsonPropertyName("sha256")]
        public string Sha256 { get; set; } = string.Empty;

        [JsonPropertyName("file_size")]
        public long FileSize { get; set; }
    }
}
