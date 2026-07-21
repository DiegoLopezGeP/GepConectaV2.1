using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using WhatsappComercial.Components.ComponentesHijo.Conversaciones;
using WhatsappComercial.Modelos.DTOs;
namespace WhatsappComercial.Hubs
{
    public class GepConectaHub : Hub
    {
        #region Conexion Hub
        private readonly ILogger<GepConectaHub> _logger;
        // Cambié Dictionary por ConcurrentDictionary: OnConnectedAsync/OnDisconnectedAsync
        // pueden dispararse concurrentemente desde distintos hilos cuando varios usuarios
        // conectan/desconectan al mismo tiempo. Un Dictionary normal no es thread-safe
        // y con 100 usuarios reales vas a tener corrupción de estado tarde o temprano.
        public static ConcurrentDictionary<string, string> UsuariosConectados = new();

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
                usuario = NormalizarUsuario(usuario);

                UsuariosConectados[Context.ConnectionId] = usuario;

                await Groups.AddToGroupAsync(Context.ConnectionId, usuario);

                _logger.LogInformation("Usuario conectado: {usuario} (ConnectionId: {connectionId})",
                    usuario, Context.ConnectionId);
            }
            else
            {
                _logger.LogWarning("Conexión sin usuario identificable: {connectionId}", Context.ConnectionId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (UsuariosConectados.TryRemove(Context.ConnectionId, out var usuario))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, usuario);
                _logger.LogInformation("Usuario desconectado: {usuario}", usuario);
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Centralizamos la normalización en un solo lugar. Si en algún punto
        // la comparas con distinta capitalización o con/sin dominio, el envío
        // al grupo silenciosamente no llega a nadie (no lanza error, solo no encuentra al grupo).
        private static string NormalizarUsuario(string usuario) =>
            usuario.Split('\\').Last().Trim().ToLowerInvariant();

        public async Task ActualizarNuevaConversacion(string usuarioDestino, DatosTarjetaConversacionDTO nuevaConversacion, bool esAsignada)
        {
            try
            {
                var grupoDestino =  (usuarioDestino);

                await Clients.Group(grupoDestino)
                    .SendAsync("ActualizarNuevaConversacion", usuarioDestino, nuevaConversacion, esAsignada);
            }
            catch (Exception ex)
            {
                // Ojo: el primer parámetro de LogError debe ser la excepción, no ex.Message.
                // Como estaba, perdías el stack trace completo en el log.
                _logger.LogError(ex, "❌ Error al enviar mensaje del BOT a {UsuarioDestino}", usuarioDestino);
            }
        }
        #endregion
    }
}
