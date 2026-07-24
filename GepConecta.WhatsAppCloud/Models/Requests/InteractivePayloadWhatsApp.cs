using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace GepConecta.WhatsAppCloud.Models.Responses
{
    public class InteractivePayloadWhatsApp : PeticionBaseWhatsApp
    {
        public InteractivePayloadWhatsApp()
        {
            type = "interactive";
        }

        [JsonProperty("interactive")]
        public required WhatsAppInteractiveBody Interactive { get; set; }
    }

    public class WhatsAppInteractiveBody
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "button";

        [JsonProperty("body")]
        public required WhatsAppTextBody Body { get; set; }

        [JsonProperty("action")]
        public required WhatsAppInteractiveAction Action { get; set; }
    }

    public class WhatsAppInteractiveAction
    {
        [JsonProperty("buttons")]
        public List<WhatsAppButtonOption>? Buttons { get; set; }
    }

    public class WhatsAppButtonOption
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "reply";

        [JsonProperty("reply")]
        public required WhatsAppReplyButton Reply { get; set; }
    }

    public class WhatsAppReplyButton
    {
        [JsonProperty("id")]
        public required string Id { get; set; }

        [JsonProperty("title")]
        public required string Title { get; set; } // Máximo 20 caracteres permitido por Meta
    }
}
