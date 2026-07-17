namespace WhatsappComercial.Interfaces.ProcesarMensajeEntrante
{
    public interface IProcesarMensajeEntrante
    {
        Task<bool> ProcesarMensajeEntrante(string mensaje, string usuario);
    }
}
