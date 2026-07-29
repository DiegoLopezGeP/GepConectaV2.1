using WhatsappComercial.Interfaces.NotificarUI;
using WhatsappComercial.Modelos.DTOs;

namespace WhatsappComercial.Servicios.NotificarUI
{
    public class NotificarUIService : INotificarUI
    {
        // Evento asíncrono para Blazor
        public event Func<int, MensajeDTO, Task>? OnNuevoMensajeEntrante;
        public event Func<int, string, string, Task>? CambiodeEstadoMensaje;

        public async Task NotificarEstadoMensajeCambiado(int idConversacion, string wamid, string nuevoEstado)
        {
            if (CambiodeEstadoMensaje != null)
            {
                var delegados = CambiodeEstadoMensaje.GetInvocationList();

                foreach (var delegado in delegados)
                {
                    try
                    {
                        var handler = (Func<int, string, string, Task>)delegado;
                        await handler.Invoke(idConversacion, wamid, nuevoEstado);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR NOTIFICADOR UI]: Fallo al notificar cambio de estado: {ex.Message}");
                    }
                }
            }
        }

        public async Task NotificarNuevoMensajeEntrante(int idConversacion, MensajeDTO nuevoMensajeUI)
        {
            if (OnNuevoMensajeEntrante != null)
            {
                // Invocar a todos los suscriptores (componentes Blazor escuchando)
                var delegados = OnNuevoMensajeEntrante.GetInvocationList();

                foreach (var delegado in delegados)
                {
                    try
                    {
                        var handler = (Func<int, MensajeDTO, Task>)delegado;
                        await handler.Invoke(idConversacion, nuevoMensajeUI);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR NOTIFICADOR UI]: Fallo al notificar a un componente Blazor: {ex.Message}");
                    }
                }
            }
        }
    }
}