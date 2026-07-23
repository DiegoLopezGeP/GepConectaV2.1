using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components.Authorization;
using WhatsappComercial.Servicios.ConfiguracionEstatica;

namespace WhatsappComercial.Servicios.SystemServicio
{
    public class SystemService
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private ConfiguracionEstaticaApp _staticConfiguracion = new ConfiguracionEstaticaApp();


        public SystemService(AuthenticationStateProvider authenticationStateProvider)
        {
            _authenticationStateProvider = authenticationStateProvider;
        }

        private string _opcionSeleccionada = "Inicio";
        public event Action? OpcionCambiada;
        public string RenombrarArchivo(string renombrar)
        {
            try
            {
                return Regex.Replace(renombrar, @"[!¡+*+~.=\/]", "_");


            }
            catch (Exception ex)
            {
                throw new Exception($"Error al renombrar el archivo {ex}");
            }
        }
        public string PathDocument()
        {
            try
            {
                string documentsPath = _staticConfiguracion.TraerConfiguracionPorCondicion("PathLocal");
                return documentsPath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }

        }
        public bool CrearCarpeta(string nombreCarpeta)
        {
            try
            {
                string rutaBase = PathDocument();
                string rutaCarpeta = Path.Combine(rutaBase, nombreCarpeta);

                if (!Directory.Exists(rutaCarpeta))
                {
                    try
                    {

                        Directory.CreateDirectory(rutaCarpeta);

                        return true;
                    }
                    catch (Exception ex)
                    {

                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public DateTime ConvercionDeStampDateTime(long fechaStamp)
        {
            DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeSeconds(fechaStamp);
            DateTime localDateTime = dateTime.LocalDateTime; // Convierte a UTC sin offset

            return localDateTime; // Devuelve DateTime sin zona horaria
        }
        public async Task<string> UserName()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var userName = authState.User.Identity?.Name.Split("\\").Last();
            return userName ?? "Usuario no autenticado";
        }
        public async Task<string> ObtenerUsuarioAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                return user.Identity.Name; // Ejemplo: "DOMINIO\usuario"
            }

            return "Usuario no autenticado";
        }
        public async Task<List<string>> ObtenerRolesAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            return roles;
        }
        public long TimestampoActual() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        public DateTime FechaActual() => DateTime.Now;
        public string OpcionSeleccionada
        {
            get => _opcionSeleccionada;
            set
            {
                if (_opcionSeleccionada != value)
                {
                    _opcionSeleccionada = value;
                    OpcionCambiada?.Invoke();
                }
            }
        }
    }
}
