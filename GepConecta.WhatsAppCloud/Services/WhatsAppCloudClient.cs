using System.Net.Http.Headers;
using System.Text;
using GepConecta.WhatsAppCloud.Models.Requests;
using GepConecta.WhatsAppCloud.Models.Responses;
using Newtonsoft.Json;

namespace GepConecta.WhatsAppCloud.Services
{
    public class WhatsAppCloudClient : IWhatsAppCloudClient
    {
        private readonly HttpClient _client;
        private readonly string _accessToken;
        private readonly string _phoneNumberId;
        private readonly string _apiVersion;

        public WhatsAppCloudClient(HttpClient httpClient, string accessToken, string phoneNumberId, string apiVersion = "v25.0")
        {
            _client = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _accessToken = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
            _phoneNumberId = phoneNumberId ?? throw new ArgumentNullException(nameof(phoneNumberId));
            _apiVersion = apiVersion;
        }

        #region MÉTODOS PÚBLICOS DE LA INTERFAZ

        // 1. ENVÍO DE TEXTO PLANO
        public async Task<EnviarRespuestaWhatsApp?> SendTextMessageAsync(string numeroTelefonoCliente, string type, string mensaje)
        {
            var payload = new TextoPayloadWhatsApp
            {
                messaging_product = "whatsapp",
                recipient_type = "individual",
                to = numeroTelefonoCliente,
                type = type,
                Text = new WhatsAppTextBody { Body = mensaje }
            };

            return await EnviarPeticionMetaAsync<EnviarRespuestaWhatsApp>("messages", payload);
        }

        // 2. ENVÍO DE AUDIO / NOTA DE VOZ
        public async Task<EnviarRespuestaWhatsApp?> SendAudioAsync(string to, string mediaIdOrUrl, bool isUrl = false)
        {
            var payload = new MediaPayloadWhatApp
            {
                messaging_product = "whatsapp",
                recipient_type = "individual",
                to = to,
                type = "audio",
                Audio = new WhatsAppMediaBody
                {
                    Id = isUrl ? null : mediaIdOrUrl,
                    Link = isUrl ? mediaIdOrUrl : null
                }
            };

            return await EnviarPeticionMetaAsync<EnviarRespuestaWhatsApp>("messages", payload);
        }
        public async Task<EnviarRespuestaWhatsApp?> SendImageAsync(string to, string mediaIdOrUrl, string? caption = null, bool isUrl = false)
        {
            var payload = new MediaPayloadWhatApp
            {
                messaging_product = "whatsapp",
                recipient_type = "individual",
                to = to,
                type = "image",
                Image = new WhatsAppMediaBody
                {
                    Id = isUrl ? null : mediaIdOrUrl,
                    Link = isUrl ? mediaIdOrUrl : null,
                    Caption = caption
                }
            };
            return await EnviarPeticionMetaAsync<EnviarRespuestaWhatsApp>("messages", payload);
        }

        public async Task<EnviarRespuestaWhatsApp?> SendDocumentAsync(string to, string mediaIdOrUrl, string? fileName = null, string? caption = null, bool isUrl = false)
        {
            var payload = new MediaPayloadWhatApp
            {
                messaging_product = "whatsapp",
                recipient_type = "individual",
                to = to,
                type = "document",
                Document = new WhatsAppMediaBody
                {
                    Id = isUrl ? null : mediaIdOrUrl,
                    Link = isUrl ? mediaIdOrUrl : null,
                    Filename = fileName,
                    Caption = caption
                }
            };

            return await EnviarPeticionMetaAsync<EnviarRespuestaWhatsApp>("messages", payload);
        }

        public async Task<EnviarRespuestaWhatsApp?> SendVideoAsync(string to, string mediaIdOrUrl, string? caption = null, bool isUrl = false)
        {
            var payload = new MediaPayloadWhatApp
            {
                messaging_product = "whatsapp",
                recipient_type = "individual",
                to = to,
                type = "video",
                Video = new WhatsAppMediaBody
                {
                    Id = isUrl ? null : mediaIdOrUrl,
                    Link = isUrl ? mediaIdOrUrl : null,
                    Caption = caption
                }
            };

            return await EnviarPeticionMetaAsync<EnviarRespuestaWhatsApp>("messages", payload);
        }

        public async Task<EnviarRespuestaWhatsApp?> SendStickerAsync(string to, string mediaIdOrUrl, bool isUrl = false)
        {
            var payload = new MediaPayloadWhatApp
            {
                messaging_product = "whatsapp",
                recipient_type = "individual",
                to = to,
                type = "sticker",
                Sticker = new WhatsAppMediaBody
                {
                    Id = isUrl ? null : mediaIdOrUrl,
                    Link = isUrl ? mediaIdOrUrl : null
                }
            };
            return await EnviarPeticionMetaAsync<EnviarRespuestaWhatsApp>("messages", payload);
        }

        // 3. SUBIDA DE ARCHIVOS MULTIMEDIA (Audios, imágenes, documentos, etc.)
        public async Task<WhatsAppMediaUploadResponse?> UploadMediaAsync(byte[] fileBytes, string fileName, string contentType)
        {
            using var content = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

            content.Add(fileContent, "file", fileName);
            content.Add(new StringContent("whatsapp"), "messaging_product");

            return await EnviarPeticionMultipartMetaAsync<WhatsAppMediaUploadResponse>("media", content);
        }

        // 4. ENVÍO DE PLANTILLAS (TEMPLATES)
        public async Task<EnviarRespuestaWhatsApp?> SendTemplateMessageAsync(string to, string templateName, string languageCode = "es")
        {
            var payload = new PlantillaPayloadWhatsApp
            {
                messaging_product = "whatsapp",
                recipient_type = "individual",
                to = to,
                type = "template",
                Template = new WhatsAppTemplateBody
                {
                    Name = templateName,
                    Language = new WhatsAppLanguage { Code = languageCode }
                }
            };

            return await EnviarPeticionMetaAsync<EnviarRespuestaWhatsApp>("messages", payload);
        }

        // 5. ENVÍO DE BOTONES INTERACTIVOS
        public async Task<EnviarRespuestaWhatsApp?> SendInteractiveButtonsAsync(string to, string bodyText, Dictionary<string, string> buttons)
        {
            var buttonOptions = buttons.Select(b => new WhatsAppButtonOption
            {
                Type = "reply",
                Reply = new WhatsAppReplyButton { Id = b.Key, Title = b.Value }
            }).ToList();

            var payload = new InteractivePayloadWhatsApp
            {
                messaging_product = "whatsapp",
                recipient_type = "individual",
                to = to,
                type = "interactive",
                Interactive = new WhatsAppInteractiveBody
                {
                    Type = "button",
                    Body = new WhatsAppTextBody { Body = bodyText },
                    Action = new WhatsAppInteractiveAction { Buttons = buttonOptions }
                }
            };

            return await EnviarPeticionMetaAsync<EnviarRespuestaWhatsApp>("messages", payload);
        }

        #endregion

        #region IMPLEMENTACIÓN EXPLÍCITA DE INTERFAZ

        // Implementación de la firma requerida por IWhatsAppCloudClient
        Task<string> IWhatsAppCloudClient.EnviarPeticionMetaAsync(string endPoint, object payload)
        {
            return EnviarPeticionMetaRawAsync(endPoint, payload);
        }

        #endregion

        #region MÉTODOS AUXILIARES DE COMUNICACIÓN HTTP CON META

        // Método genérico tipado para peticiones JSON (Messages, Templates, Interactive, etc.)
        private async Task<T?> EnviarPeticionMetaAsync<T>(string endPoint, object payload)
        {
            string responseJson = await EnviarPeticionMetaRawAsync(endPoint, payload);

            if (string.IsNullOrWhiteSpace(responseJson))
                return default;

            return JsonConvert.DeserializeObject<T>(responseJson);
        }

        // Método base que realiza el POST de JSON a la API Graph de Meta
        private async Task<string> EnviarPeticionMetaRawAsync(string endPoint, object payload)
        {
            try
            {
                string jsonBody = JsonConvert.SerializeObject(payload);

                Console.WriteLine($"[JSON ENVIADO A META]: {jsonBody}");

                using var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                string url = $"https://graph.facebook.com/{_apiVersion}/{_phoneNumberId}/{endPoint}";

                using var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = content
                };

                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);

                var response = await _client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    string errorDetail = await response.Content.ReadAsStringAsync();
return errorDetail;
                }

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al mandar el mensaje a WhatsApp: {ex.Message}", ex);
            }
        }

        // Método genérico para la subida de multimedia mediante multipart/form-data
        private async Task<T?> EnviarPeticionMultipartMetaAsync<T>(string endPoint, MultipartFormDataContent content)
        {
            try
            {
                string url = $"https://graph.facebook.com/{_apiVersion}/{_phoneNumberId}/{endPoint}";

                using var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = content
                };

                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);

                var response = await _client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    string errorDetail = await response.Content.ReadAsStringAsync();
                    throw new Exception($"[META UPLOAD ERROR HTTP {(int)response.StatusCode}]: {errorDetail}");
                }

                string responseJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseJson);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al subir archivo a WhatsApp: {ex.Message}", ex);
            }
        }

        public Task<EnviarRespuestaWhatsApp?> SendMediaMessageAsync(string to, string mediaType, string mediaIdOrUrl, string? caption = null, string? fileName = null, bool isUrl = false)
        {
            string typeLower = mediaType.ToLower().Trim();

            var mediaBody = new WhatsAppMediaBody
            {
                Id = isUrl ? null : mediaIdOrUrl,
                Link = isUrl ? mediaIdOrUrl : null,
                Caption = (typeLower == "audio" || typeLower == "sticker") ? null : caption, // Audio/Sticker no aceptan caption
                Filename = typeLower == "document" ? fileName : null
            };

            var payload = new MediaPayloadWhatApp
            {
                messaging_product = "whatsapp",
                recipient_type = "individual",
                to = to,
                type = typeLower
            };

            // Asignamos la propiedad adecuada según el tipo de archivo
            switch (typeLower)
            {
                case "image":
                    payload.Image = mediaBody;
                    break;
                case "document":
                    payload.Document = mediaBody;
                    break;
                case "audio":
                    payload.Audio = mediaBody;
                    break;
                case "video":
                    payload.Video = mediaBody;
                    break;
                case "sticker":
                    payload.Sticker = mediaBody;
                    break;
                default:
                    throw new ArgumentException($"Tipo de archivo no soportado: '{mediaType}'. Tipos válidos: image, document, audio, video, sticker.");
            }

            return EnviarPeticionMetaAsync<EnviarRespuestaWhatsApp>("messages", payload);
        }

        public async Task<byte[]> DescargarMediaAsync(string mediaId)
        {
            if (string.IsNullOrEmpty(mediaId))
                return Array.Empty<byte>();

            try
            {
                // PASO 1: Consultar a Meta por los metadatos del MediaId para obtener la URL de descarga
                string requestUrl = $"https://graph.facebook.com/v18.0/{mediaId}";

                using var requestMeta = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                requestMeta.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

                var responseMeta = await _client.SendAsync(requestMeta);
                responseMeta.EnsureSuccessStatusCode();

                string jsonResponse = await responseMeta.Content.ReadAsStringAsync();
                var mediaInfo = JsonConvert.DeserializeObject<MetaMediaResponse>(jsonResponse);

                if (mediaInfo == null || string.IsNullOrEmpty(mediaInfo.Url))
                {
                    Console.WriteLine($"[ERROR MEDIA]: No se pudo obtener la URL para el MediaID '{mediaId}'.");
                    return Array.Empty<byte>();
                }

                // PASO 2: Descargar el archivo binario usando la URL obtenida
                using var requestDownload = new HttpRequestMessage(HttpMethod.Get, mediaInfo.Url);
                requestDownload.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

                // Importante: Agregar User-Agent para evitar que Meta rechace la petición de descarga
                requestDownload.Headers.UserAgent.ParseAdd("WhatsAppCloudClient/1.0");

                var responseDownload = await _client.SendAsync(requestDownload);
                responseDownload.EnsureSuccessStatusCode();

                // Retornar los bytes del archivo en memoria
                return await responseDownload.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR DESCARGA MEDIA META]: {ex.Message}");
                return Array.Empty<byte>();
            }
        }

        public async Task<PlantillaMetaResponseDTO?> ObtenerContenidoPlantillaAsync(string IdPlantillaMeta)
        {
            if (string.IsNullOrWhiteSpace(IdPlantillaMeta))
                return null;

            try
            {
                string idEscaped = Uri.EscapeDataString(IdPlantillaMeta.Trim());
                string requestUrl = $"https://graph.facebook.com/v25.0/{idEscaped}";

                using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

                var response = await _client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[META ERROR CONSULTA PLANTILLA {response.StatusCode}]: {errorContent}");
                    return null;
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();

                // Deserializamos DIRECTAMENTE la plantilla (ya no usamos Wrapper)
                var plantillaDetalle = JsonConvert.DeserializeObject<PlantillaMetaResponseDTO>(jsonResponse);
                return plantillaDetalle;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXCEPCIÓN PLANTILLA META]: Error al consultar la plantilla '{IdPlantillaMeta}': {ex.Message}");
                return null;
            }
        }

        public async Task<EnviarMensajeResponseDTO?> EnviarPlantillaAsync(EnviarMensajePlantillaRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Para) || string.IsNullOrWhiteSpace(request.Plantilla.Nombre))
                return null;

            try
            {
                // Se usa la URL con PHONE_NUMBER_ID
                string requestUrl = $"https://graph.facebook.com/v25.0/{_phoneNumberId}/messages";

                string jsonPayload = JsonConvert.SerializeObject(request, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                using var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUrl);
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                httpRequest.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await _client.SendAsync(httpRequest);
                string jsonResponse = await response.Content.ReadAsStringAsync();

                var respuestaMeta = JsonConvert.DeserializeObject<EnviarMensajeResponseDTO>(jsonResponse);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[META ERROR ENVÍO PLANTILLA {response.StatusCode}]: {jsonResponse}");
                }

                return respuestaMeta;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXCEPCIÓN AL ENVIAR PLANTILLA META]: {ex.Message}");
                return null;
            }
        }

        // Clase envoltorio necesaria porque Meta retorna un arreglo "data" al consultar message_templates
        internal class MetaTemplatesResponseWrapper
        {
            [JsonProperty("data")]
            public List<PlantillaMetaResponseDTO>? Data { get; set; }
        }



        #endregion
    }
}