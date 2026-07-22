using WhatsappComercial.Interfaces.Mensajes;
using WhatsappComercial.Modelos;
using WhatsappComercial.Modelos.DTOs;
using WhatsappComercial.Servicios.AccesoADatos;

namespace WhatsappComercial.Servicios.Mensajes
{

    public class EnviarMensajeService : IEnviarMensaje
    {
        private readonly ServicioAccesoDatos _servicioAccesoDatos;

        public EnviarMensajeService(ServicioAccesoDatos servicioAccesoDatos)
        {
            _servicioAccesoDatos = servicioAccesoDatos;
        }


        public async Task<int> EnviarMensajeAsync(int idConversacion, string nombreUsuario, string celular, MensajeDTO nuevoMensaje)
        {
            var entidadMensaje = new Modelos.Mensajes
            {
                IdConversacion = idConversacion,
                ContenidoMensaje = nuevoMensaje.Texto,
                FechaEnvioMensaje = nuevoMensaje.Fecha,
                MensajeEntranteMensaje = nuevoMensaje.EsEntrante,
                EsBot = nuevoMensaje.EsEntrante,
                NumTelefonoCliente = celular,
                NombreUsuarioMensaje = nombreUsuario,
                TipoMensaje = "text",
                Leido = false,
                EstadoEnvio = "send" // Estado inicial: p. ej., "Enviado"
                                     // Asigna los demás campos que requiera tu tabla de BD
            };

            string idMensajeRegistrado = _servicioAccesoDatos.GrabarRegistroDevuelveConsecutivo(entidadMensaje, "Mensajes");

            return Convert.ToInt32(idMensajeRegistrado);

        }
    }
}
