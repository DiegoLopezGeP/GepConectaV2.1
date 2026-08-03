using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace GepConecta.WhatsAppCloud.Models.Responses
{
    public class MensajeInfoResponseDTO
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;
    }
}
