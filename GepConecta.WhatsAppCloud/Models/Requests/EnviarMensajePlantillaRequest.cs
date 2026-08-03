using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace GepConecta.WhatsAppCloud.Models.Requests
{
    public class EnviarMensajePlantillaRequest
    {
        [JsonProperty("messaging_product")]
        public string MessagingProduct { get; set; } = "whatsapp";

        [JsonProperty("recipient_type")]
        public string RecipientType { get; set; } = "individual";

        [JsonProperty("to")]
        public string Para { get; set; } = string.Empty; // Número destino con código de país (ej. "573001234567")

        [JsonProperty("type")]
        public string Tipo { get; set; } = "template";

        [JsonProperty("template")]
        public PlantillaEnvioDTO Plantilla { get; set; } = new();
    }
}
