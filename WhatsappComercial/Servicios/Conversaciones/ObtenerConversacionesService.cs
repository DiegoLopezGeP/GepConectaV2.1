using System.Data;
using WhatsappComercial.Interfaces.Conversaciones;
using WhatsappComercial.Modelos;
using WhatsappComercial.Modelos.DTOs;
using WhatsappComercial.Servicios.AccesoADatos;

namespace WhatsappComercial.Servicios.Conversaciones
{
    public class ObtenerConversacionesService : IObtenerConversaciones
    {
        private readonly ServicioAccesoDatos _servicioAccesoDatos;

        public ObtenerConversacionesService(ServicioAccesoDatos servicioAccesoDatos)
        {
            _servicioAccesoDatos = servicioAccesoDatos;
        }

        public async Task<List<DatosTarjetaConversacionDTO>> ObtenerDatosConversacionContacto(GrupoUser informacionUsuarioAutenticado)
        {
            List<DatosTarjetaConversacionDTO> listaConversaciones = new List<DatosTarjetaConversacionDTO>();
            string[] parametros = {
                informacionUsuarioAutenticado.NameUser.ToString(),
                informacionUsuarioAutenticado.IdGrupoTrabajo.ToString()
            };

            DataTable dtListaConversaciones = await _servicioAccesoDatos.TraerTablaConArregloAsincrono(58, parametros);

            foreach (DataRow row in dtListaConversaciones.Rows)
            {
                listaConversaciones.Add(new DatosTarjetaConversacionDTO
                {
                    NombreCliente = row["NombreCliente"].ToString(),
                    IdTicket = Convert.ToInt32(row["IdTicket"]),
                    Fecha = DateTime.Parse(row?["Fecha"]?.ToString()),
                    EstadoConversacion = row["EstadoConversacion"].ToString()
                });
            }

            return listaConversaciones;
        }
    }
}
