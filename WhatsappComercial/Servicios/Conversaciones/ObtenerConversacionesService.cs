using System.Data;
using WhatsappComercial.Components.ComponentesHijo.Conversaciones;
using WhatsappComercial.Enums;
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

        public async Task<List<DatosTarjetaConversacionDTO>> ObtenerConversacionesActivas()
        {
            try
            {
                List<DatosTarjetaConversacionDTO> listaConversacionesActivas = new List<DatosTarjetaConversacionDTO>();

                DataTable dtListaConversaciones = await _servicioAccesoDatos.TraerTablaConArregloAsincrono(58, []);

                foreach (DataRow row in dtListaConversaciones.Rows)
                {
                    listaConversacionesActivas.Add(new DatosTarjetaConversacionDTO
                    {
                        IdConversacion = Convert.ToInt32(row["IdConversacion"]),
                        NombreCliente = row["NombreCliente"].ToString(),
                        IdTicket = Convert.ToInt32(row["IdTicket"]),
                        Fecha = DateTime.Parse(row?["Fecha"]?.ToString()),
                        EstadoConversacion = Enum.TryParse<EstadoConversacionEnum>(row["EstadoConversacion"]?.ToString(), true, out var estadoConversacion) ? estadoConversacion : EstadoConversacionEnum.Activo,
                        nombreAsesor = row["NombreAsesor"].ToString()
                    });
                }

                return listaConversacionesActivas;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<DatosContactoConversacionDTO?> ObtenerDatosContactoConversacionSeleccionada(int idTicket)
        {
            DataTable dtDatosContacto = await _servicioAccesoDatos.TraerTablaConArregloAsincrono(59, [idTicket.ToString()]);

            if (dtDatosContacto.Rows.Count == 0)
                return null;

            DataRow row = dtDatosContacto.Rows[0];

            return new DatosContactoConversacionDTO
            {
                NombreCliente = row["NombreCliente"]?.ToString(),
                IdTicket = Convert.ToInt32(row["IdTicket"]),
                FechaInicioConversacion = row["FechaInicio"] == DBNull.Value ? null : Convert.ToDateTime(row["FechaInicio"]),
                FechaFinConversacion = row["FechaFin"] == DBNull.Value ? null : Convert.ToDateTime(row["FechaFin"]),
                EstadoConversacion = Enum.TryParse<EstadoConversacionEnum>(row["EstadoConversacion"]?.ToString(), true, out var estado) ? estado : EstadoConversacionEnum.Activo,
                Celular = row["Celular"]?.ToString(),
               Identificacion = row["Identificacion"]?.ToString()
            };
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
                    EstadoConversacion = Enum.TryParse<EstadoConversacionEnum>(row["EstadoConversacion"]?.ToString(), true, out var estadoConversacion) ? estadoConversacion : EstadoConversacionEnum.Activo
                });
            }

            return listaConversaciones;
        }
    }
}
