using Microsoft.AspNetCore.SignalR;
using WhatsappComercial.Components.ComponentesHijo.Conversaciones;
using WhatsappComercial.Modelos.DTOs;
namespace WhatsappComercial.Hubs
{
    public class GepConectaHub : Hub
    {
        #region Conexion Hub
        private readonly ILogger<GepConectaHub> _logger;
        public static Dictionary<string, string> UsuariosConectados = new();

        public GepConectaHub(ILogger<GepConectaHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var http = Context.GetHttpContext();

            var usuario =
                http?.Request.Query["usuario"].ToString()
                ?? Context.UserIdentifier
                ?? Context.User?.Identity?.Name;

            if (!string.IsNullOrEmpty(usuario))
            {
                usuario = usuario.Split('\\').Last().ToLower();

                UsuariosConectados[Context.ConnectionId] = usuario;

                await Groups.AddToGroupAsync(Context.ConnectionId, usuario);

                _logger.LogInformation("Usuario conectado: {usuario}", usuario);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (UsuariosConectados.TryGetValue(Context.ConnectionId, out var usuario))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, usuario);

                UsuariosConectados.Remove(Context.ConnectionId);

                _logger.LogInformation("Usuario desconectado: {usuario}", usuario);
            }

            await base.OnDisconnectedAsync(exception);
        }
        #endregion

        #region Actualizar nueva conversación
        public async Task ActualizarNuevaConversacion(string usuarioDestino, DatosTarjetaConversacionDTO nuevaConversacion, bool esAsignada)
        {
            try
            {
                await Clients.User(usuarioDestino).SendAsync("ActualizarNuevaConversacion", usuarioDestino, nuevaConversacion, esAsignada);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "❌ Error al enviar mensaje del BOT a {UsuarioDestino}", usuarioDestino);
            }
        }
        #endregion
    }
}
