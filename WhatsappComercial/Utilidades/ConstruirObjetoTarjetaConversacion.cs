using System.Collections.Specialized;
using WhatsappComercial.Enums;
using WhatsappComercial.Interfaces.Utilidades;
using WhatsappComercial.Modelos.DTOs;

namespace WhatsappComercial.Utilidades
{
    public class ConstruirObjetoTarjetaConversacion : IUtilidades
    {
        public ConstruirObjetoTarjetaConversacion()
        {

        }
        public async Task<DatosTarjetaConversacionDTO> ConstruirObjetoTarjetaConversacionAsync(int idConversacion, int idTicket, string nombreCliente, EstadoConversacionEnum estadoConversacion, DateTime fecha, string nombreUsuario, bool esNuevaConversacion)
        {
            DatosTarjetaConversacionDTO objetoConstruido = new()
            {
                IdConversacion = idConversacion,
                NombreCliente = nombreCliente,
                IdTicket = idTicket,
                Fecha = fecha,
                EstadoConversacion = estadoConversacion,
                nombreAsesor = nombreUsuario,
                esNuevaConversacion = esNuevaConversacion

            };
            return objetoConstruido;
        }
    }
}
