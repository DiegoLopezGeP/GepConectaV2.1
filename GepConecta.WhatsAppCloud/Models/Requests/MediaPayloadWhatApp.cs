using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using GepConecta.WhatsAppCloud.Models.Requests;
using Newtonsoft.Json;

namespace GepConecta.WhatsAppCloud.Models.Responses
{
    public class MediaPayloadWhatApp : PeticionBaseWhatsApp
    {
        [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
        public WhatsAppMediaBody? Image { get; set; }

        [JsonProperty("document", NullValueHandling = NullValueHandling.Ignore)]
        public WhatsAppMediaBody? Document { get; set; }

        [JsonProperty("audio", NullValueHandling = NullValueHandling.Ignore)]
        public WhatsAppMediaBody? Audio { get; set; }

        [JsonProperty("video", NullValueHandling = NullValueHandling.Ignore)]
        public WhatsAppMediaBody? Video { get; set; }

        [JsonProperty("sticker", NullValueHandling = NullValueHandling.Ignore)]
        public WhatsAppMediaBody? Sticker { get; set; }
    }
}
