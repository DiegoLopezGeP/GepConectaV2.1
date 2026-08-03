using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace GepConecta.WhatsAppCloud.Models.Requests
{
    public class PlantillaEnvioDTO
    {
        [JsonProperty("name")]
        public string Nombre { get; set; } = string.Empty;

        [JsonProperty("language")]
        public LenguajePlantillaDTO Lenguaje { get; set; } = new();

        [JsonProperty("components")]
        public List<ComponenteEnvioDTO> Componentes { get; set; } = new();
    }
}
