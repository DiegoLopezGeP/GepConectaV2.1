using System.Text.Json;

namespace WhatsappComercial.Servicios.AutenticacionUsuario
{
    public class ObtenerJWT
    {
        private readonly HttpClient _httpClient;
        private readonly string _UrlAPI;

        public ObtenerJWT(HttpClient httpClient, string UrlAPI)
        {
            _httpClient = httpClient;
            _UrlAPI = UrlAPI;
        }




        public async Task<string> ObtenerToken()
        {
            try
            {
                string DireccionApi = _UrlAPI;
                string direccionApiNueva = DireccionApi.Replace("/api/", "");
                string usuarioJWT = "UserApi";
                string passwordJWT = "GEP2024*+";

                var credencialesJWT = new
                {
                    username = usuarioJWT,
                    password = passwordJWT
                };


                var respuesta = _httpClient.PostAsJsonAsync($"{direccionApiNueva}/Login/Autenticar", credencialesJWT).GetAwaiter().GetResult();

                if (respuesta.IsSuccessStatusCode)
                {

                    var json = await respuesta.Content.ReadAsStringAsync();

                    // Deserializar el JSON para obtener el token
                    var tokenResponse = JsonSerializer.Deserialize<TokenRespuesta>(json);

                    // Devolver solo el valor del token
                    return tokenResponse.token;
                }
                else
                {
                    return $"Error: {respuesta.StatusCode} - {respuesta.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return $"Error: {ex.Message}";
            }
        }
        public class TokenRespuesta
        {
            public string token { get; set; }
        }
    }
}

