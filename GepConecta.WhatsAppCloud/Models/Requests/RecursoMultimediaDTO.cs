using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace GepConecta.WhatsAppCloud.Models.Requests
{
    public class RecursoMultimediaDTO
    {
        [JsonProperty("link")]
        public string? Link { get; set; } // URL pública de la imagen o documento (https://...)

        [JsonProperty("id")]
        public string? Id { get; set; } // O el Media ID de WhatsApp

        [JsonProperty("filename")]
        public string? NombreArchivo { get; set; }
    }
}
