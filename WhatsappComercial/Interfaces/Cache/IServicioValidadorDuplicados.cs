namespace WhatsappComercial.Interfaces.Cache
{
    public interface IServicioValidadorDuplicados
    {
        bool EsMensajeDuplicado(string wamid);
        void RegistrarMensajeProcesado(string wamid);
    }
}
