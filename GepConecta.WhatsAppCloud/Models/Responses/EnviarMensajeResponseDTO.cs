using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace GepConecta.WhatsAppCloud.Models.Responses
{
    public class EnviarMensajeResponseDTO
    {
        [JsonProperty("messaging_product")]
        public string MessagingProduct { get; set; } = string.Empty;

        [JsonProperty("contacts")]
        public List<ContactoResponseDTO>? Contactos { get; set; }

        [JsonProperty("messages")]
        public List<MensajeInfoResponseDTO>? Mensajes { get; set; }

        [JsonProperty("error")]
        public ErrorMetaDTO? Error { get; set; }
    }
}
