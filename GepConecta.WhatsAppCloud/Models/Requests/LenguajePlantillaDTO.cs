using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace GepConecta.WhatsAppCloud.Models.Requests
{
    public class LenguajePlantillaDTO
    {
        [JsonProperty("code")]
        public string Codigo { get; set; } = "es_CO"; // es, es_LA, es_CO, en_US, etc.
    }
}
