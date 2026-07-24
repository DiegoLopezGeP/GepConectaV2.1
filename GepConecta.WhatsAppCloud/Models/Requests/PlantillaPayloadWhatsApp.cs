using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace GepConecta.WhatsAppCloud.Models.Responses
{
    public class PlantillaPayloadWhatsApp : PeticionBaseWhatsApp
    {
        public PlantillaPayloadWhatsApp()
        {
            type = "template";
        }

        [JsonProperty("template")]
        public required WhatsAppTemplateBody Template { get; set; }
    }

    public class WhatsAppTemplateBody
    {
        [JsonProperty("name")]
        public required string Name { get; set; }

        [JsonProperty("language")]
        public WhatsAppLanguage Language { get; set; } = new WhatsAppLanguage();

        [JsonProperty("components")]
        public List<WhatsAppTemplateComponent>? Components { get; set; }
    }

    public class WhatsAppLanguage
    {
        [JsonProperty("code")]
        public string Code { get; set; } = "es";
    }

    public class WhatsAppTemplateComponent
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "body";

        [JsonProperty("parameters")]
        public List<WhatsAppTemplateParameter> Parameters { get; set; } = new();
    }

    public class WhatsAppTemplateParameter
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "text";

        [JsonProperty("text")]
        public string? Text { get; set; }
    }
}
