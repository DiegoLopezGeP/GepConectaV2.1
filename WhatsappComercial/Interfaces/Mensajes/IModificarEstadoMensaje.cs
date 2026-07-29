namespace WhatsappComercial.Interfaces.Mensajes
{
    public interface IModificarEstadoMensaje
    {
        Task ActualizarEstadoMensaje(int idConversacion, string waid, string nuevoEstado);
    }
}
