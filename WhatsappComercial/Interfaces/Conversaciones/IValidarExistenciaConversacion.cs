namespace WhatsappComercial.Interfaces.Conversaciones
{
    public interface IValidarExistenciaConversacion
    {
        Task<int> ExisteConversacion(string numeroTelefonoConversacion);
    }
}
