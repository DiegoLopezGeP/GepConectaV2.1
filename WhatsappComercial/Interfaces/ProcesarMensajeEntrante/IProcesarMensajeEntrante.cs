namespace WhatsappComercial.Interfaces.ProcesarMensajeEntrante
{
    public interface IProcesarMensajeEntrante
    {
        Task ProcesarMensajeEntrante(string mensaje, string usuario);
    }
}
