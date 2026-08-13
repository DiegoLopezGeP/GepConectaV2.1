using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GepConecta.WhatsAppCloud.Models.Webhook
{
    public class WebhookPayloadWhatsApp
    {
        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("entry")]
        public List<WebhookEntry>? Entry { get; set; }
    }

    public class WebhookEntry
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("changes")]
        public List<WebhookChange>? Changes { get; set; }
    }

    public class WebhookChange
    {
        [JsonPropertyName("field")]
        public string? Field { get; set; }

        [JsonPropertyName("value")]
        public WebhookValue? Value { get; set; }
    }

    public class WebhookValue
    {
        [JsonPropertyName("messaging_product")]
        public string? MessagingProduct { get; set; }

        [JsonPropertyName("metadata")]
        public WebhookMetadata? Metadata { get; set; }

        [JsonPropertyName("contacts")]
        public List<WebhookContact>? Contacts { get; set; }

        [JsonPropertyName("messages")]
        public List<WebhookMessage>? Messages { get; set; }

        [JsonPropertyName("statuses")]
        public List<WebhookStatus>? Statuses { get; set; }
    }

    public class WebhookMetadata
    {
        [JsonPropertyName("display_phone_number")]
        public string? DisplayPhoneNumber { get; set; }

        [JsonPropertyName("phone_number_id")]
        public string? PhoneNumberId { get; set; }
    }

    public class WebhookContact
    {
        [JsonPropertyName("profile")]
        public WebhookProfile? Profile { get; set; }

        [JsonPropertyName("wa_id")]
        public string? WaId { get; set; }

        [JsonPropertyName("user_id")]
        public string? UserId { get; set; }
    }

    public class WebhookProfile
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class WebhookMessage
    {
        [JsonPropertyName("from")]
        public string? From { get; set; }

        [JsonPropertyName("from_user_id")]
        public string? FromUserId { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("timestamp")]
        public string? Timestamp { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; } // "text", "image", "audio", "document", "video", etc.

        // Tipos de contenido
        [JsonPropertyName("text")]
        public WebhookTextBody? Text { get; set; }

        [JsonPropertyName("image")]
        public WebhookMediaBody? Image { get; set; }

        [JsonPropertyName("audio")]
        public WebhookMediaBody? Audio { get; set; }

        [JsonPropertyName("document")]
        public WebhookMediaBody? Document { get; set; }

        [JsonPropertyName("video")]
        public WebhookMediaBody? Video { get; set; }

        [JsonPropertyName("context")]
        public WebhookContextBody? Context { get; set; } // Si responde a un mensaje específico
    }

    public class WebhookTextBody
    {
        [JsonPropertyName("body")]
        public string? Body { get; set; }
    }

    public class WebhookMediaBody
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("mime_type")]
        public string? MimeType { get; set; }

        [JsonPropertyName("sha256")]
        public string? Sha256 { get; set; }

        [JsonPropertyName("caption")]
        public string? Caption { get; set; }

        [JsonPropertyName("filename")]
        public string? Filename { get; set; }

        [JsonPropertyName("voice")]
        public bool? Voice { get; set; } // Indica si es una nota de voz de WhatsApp
    }

    public class WebhookContextBody
    {
        [JsonPropertyName("from")]
        public string? From { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; } // Id del mensaje original que el usuario citó
    }

    public class WebhookStatus
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; } // "sent", "delivered", "read", "failed"

        // ✅ CORREGIDO: Ahora es una Lista de errores
        [JsonPropertyName("errors")]
        public List<WebhookStatusErrors>? Errors { get; set; }

        [JsonPropertyName("timestamp")]
        public string? Timestamp { get; set; }

        [JsonPropertyName("recipient_id")]
        public string? RecipientId { get; set; }
    }
    public class WebhookStatusErrors
    {
        [JsonPropertyName("code")]
        public int? Code { get; set; } // Meta lo envía como entero (ej. 131026)

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("error_data")]
        public object? ErrorData { get; set; } // O JsonElement? por si Meta envía un objeto complejo en error_data
    }
}
