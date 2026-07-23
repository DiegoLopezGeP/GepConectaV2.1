using System.Data;
using WhatsappComercial.Interfaces.Mensajes;
using WhatsappComercial.Modelos.DTOs;
using WhatsappComercial.Servicios.AccesoADatos;

namespace WhatsappComercial.Servicios.Mensajes
{
    public class ObtenerMensajesConversacionService : IObtenerMensajesConversacion
    {
        private readonly ServicioAccesoDatos _servicioAccesoDatos;

        public ObtenerMensajesConversacionService(ServicioAccesoDatos servicioAccesoDatos)
        {
            _servicioAccesoDatos = servicioAccesoDatos;
        }

        public async Task<List<MensajeDTO>> MensajesConversacion(int idConversacion, int cantidad, string? idMensajeCursor)
        {
            List<MensajeDTO> listaMensajesConversacion = new();

            // @v1 = cantidad, @v2 = idConversacion, @v3 = cursor (null en la primera carga)
            string?[] parametros = [ cantidad.ToString(), idConversacion.ToString(),idMensajeCursor?.ToString() // si es null, queda null en el arreglo
            ];

            DataTable dtMensajes = await _servicioAccesoDatos.TraerTablaConArregloAsincrono(60, parametros);

            // El SP trae ordenado DESC (más nuevo primero) por eficiencia del índice;
            // lo invertimos aquí para que la UI reciba orden cronológico ascendente.
            listaMensajesConversacion = dtMensajes.AsEnumerable()
                .Select(row => new MensajeDTO
                {
                    IdMensaje = row.Field<int>("IdMensaje"),
                    Texto = row.Field<string>("Texto"),
                    Fecha = row.Field<DateTime>("Fecha"),
                    EsEntrante = row.Field<bool>("EsEntrante"),
                    TipoArchivo = row.Field<string>("TipoMensaje"),
                    HashSha256 = row.Field<string>("HashSHA256")
                })
                .OrderBy(m => m.IdMensaje)
                .ToList();

            return listaMensajesConversacion;
        }
    }
}