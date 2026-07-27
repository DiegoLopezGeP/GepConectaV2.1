using WhatsappComercial.Interfaces.NotificarUI;
using WhatsappComercial.Modelos.DTOs;

namespace WhatsappComercial.Servicios.NotificarUI
{
    public class NotificarUIService : INotificarUI
    {
        // Evento asíncrono para Blazor
        public event Func<int, MensajeDTO, Task>? OnNuevoMensajeEntrante;

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