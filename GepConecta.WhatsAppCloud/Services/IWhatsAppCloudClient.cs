using System;
using System.Collections.Generic;
using System.Text;
using GepConecta.WhatsAppCloud.Models.Responses;

namespace GepConecta.WhatsAppCloud.Services
{
    public interface IWhatsAppCloudClient
    {
        Task<string> EnviarPeticionMetaAsync(string endPoint, object payload);
        Task<EnviarRespuestaWhatsApp?> SendTextMessageAsync(string numeroTelefonoCliente,string type, string mensaje);
        Task<EnviarRespuestaWhatsApp?> SendAudioAsync(string to, string mediaIdOrUrl, bool isUrl = false);
        Task<WhatsAppMediaUploadResponse?> UploadMediaAsync(byte[] fileBytes, string fileName, string contentType);
        Task<EnviarRespuestaWhatsApp?> SendImageAsync(string to, string mediaIdOrUrl, string? caption = null, bool isUrl = false);
        Task<EnviarRespuestaWhatsApp?> SendDocumentAsync(string to, string mediaIdOrUrl, string? fileName = null, string? caption = null, bool isUrl = false);
        Task<EnviarRespuestaWhatsApp?> SendVideoAsync(string to, string mediaIdOrUrl, string? caption = null, bool isUrl = false);
        Task<EnviarRespuestaWhatsApp?> SendStickerAsync(string to, string mediaIdOrUrl, bool isUrl = false);
        Task<EnviarRespuestaWhatsApp?> SendTemplateMessageAsync(string to, string templateName, string languageCode = "es");
        Task<EnviarRespuestaWhatsApp?> SendInteractiveButtonsAsync(string to, string bodyText, Dictionary<string, string> buttons);

        Task<EnviarRespuestaWhatsApp?> SendMediaMessageAsync(string to, string mediaType, string mediaIdOrUrl, string? caption = null, string? fileName = null, bool isUrl = false);

        Task<byte[]> DescargarMediaAsync(string mediaId);
    }
}
