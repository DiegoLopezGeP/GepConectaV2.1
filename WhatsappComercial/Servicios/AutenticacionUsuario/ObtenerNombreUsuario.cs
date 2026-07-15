using System.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using RestSharp;
using WhatsappComercial.Modelos;
using WhatsappComercial.Servicios.AccesoADatos;
using Serilog;

namespace WhatsappComercial.Servicios.AutenticacionUsuario
{
    public class ObtenerNombreUsuario
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ConfiguracionApp configuracionAplicacion;
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly ServicioAccesoDatos capaDatos;


        public dynamic TokenRecibido { get; set; }

        public string DireccionApi { get; set; }

        public string LoginUsuario { get; set; }

        public string? NombreCompleto { get; set; }
        public string? IdUsuario { get; set; }
        public string? FotoUsuario { get; set; }
        public string? IdProvedor { get; set; }
        public string? Extension { get; set; }

        public string? Area { get; set; }

        public ObtenerNombreUsuario(AuthenticationStateProvider authenticationStateProvider, IOptions<ConfiguracionApp> options, ServicioAccesoDatos datos)
        {
            _authenticationStateProvider = authenticationStateProvider;
            configuracionAplicacion = options.Value;
            capaDatos = datos;
            ObtenerUsuarioActualAsync();
        }

        public async void ObtenerUsuarioActualAsync()
        {
            ObtenerJWT _obtenerJWT;
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            LoginUsuario = authState.User.Identity?.Name.Replace("GEPDC\\", "");

            _obtenerJWT = new ObtenerJWT(_httpClient, configuracionAplicacion.ApiUsuarios);
            DireccionApi = configuracionAplicacion.ApiUsuarios;
            TokenRecibido = _obtenerJWT.ObtenerToken().GetAwaiter().GetResult();

            await DatosDeUsuario(LoginUsuario, DireccionApi, TokenRecibido);
        }


        public async Task DatosDeUsuario(string? usuario, string? direccionApi, string? tokenRecibido)
        {
            Log.Logger = new LoggerConfiguration()
                            .WriteTo.File("logs/eventosCRM_DatosdeUsuario.txt", rollingInterval: RollingInterval.Day)
                            .CreateLogger();

            try
            {
                RestClient cliente = new RestClient(direccionApi + "usuarios?usuario=" + usuario);
                cliente.AddDefaultHeader("Authorization", "Bearer " + tokenRecibido);
                RestRequest request1 = new RestRequest();
                var response = cliente.Get(request1);
                string? respuesta = response.Content;
                List<DatosUsuario>? RespuestaDatos = System.Text.Json.JsonSerializer.Deserialize<List<DatosUsuario>>(respuesta);

                NombreCompleto = RespuestaDatos[0].Usuarios_Nombre;
                FotoUsuario = RespuestaDatos[0].Usuarios_Foto.ToString();
                string identificadorUsuario = RespuestaDatos[0].Usuarios_IdUsuario.ToString();

                capaDatos.Aplicacion = int.Parse(configuracionAplicacion.Aplicacion);

                string[] Parametros = { identificadorUsuario };
                DataTable dtDatosUsuarios = capaDatos.TraerTablaConArreglo(41, Parametros);

                IdUsuario = dtDatosUsuarios.Rows[0]["IdUsuario"].ToString();
                IdProvedor = dtDatosUsuarios.Rows[0]["IdProveedor"].ToString();
                Extension = dtDatosUsuarios.Rows[0]["Extension"].ToString();
                Area = dtDatosUsuarios.Rows[0]["Area"].ToString();
            }
            catch (Exception ex)
            {
                Log.Information($"Error al obtener datos de la API usuarios: {ex.Message}");
            }
        }


        public string NombreUsuario()
        {
            return NombreCompleto;
        }

        public string IdentificadorUsuario()
        {
            return IdUsuario;
        }
    }
}
