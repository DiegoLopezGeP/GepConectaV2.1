using System.Text.Json.Serialization;
using GepConecta.WhatsAppCloud.Models.Responses;
using Newtonsoft.Json;

public class TextoPayloadWhatsApp : PeticionBaseWhatsApp
{
    public TextoPayloadWhatsApp()
    {
        type = "text";
        messaging_product = "whatsapp";
        recipient_type = "individual";
    }

    [JsonProperty("text")]
    public required WhatsAppTextBody Text { get; set; }
}

public class WhatsAppTextBody
{
    [JsonProperty("preview_url")]
    public bool PreviewUrl { get; set; } = false;

    [JsonProperty("body")]
    public required string Body { get; set; }
}
