using WhatsappComercial.Interfaces.ProcesarMensajeEntrante;

namespace WhatsappComercial.Servicios.ProcesarMensaje
{
    public class ProcesarMensajeService : IProcesarMensajeEntrante
    {
        public Task<bool> ProcesarMensajeEntrante(string mensaje, string usuario)
        {
            throw new NotImplementedException();
        }
    }
}
