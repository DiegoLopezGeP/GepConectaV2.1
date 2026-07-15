using System.Data;
using WhatsappComercial.Interfaces.Usuarios;
using WhatsappComercial.Modelos;
using WhatsappComercial.Servicios.AccesoADatos;

namespace WhatsappComercial.Servicios.Usuarios
{
    public class Usuarios : IUsuarios
    {

        private readonly ServicioAccesoDatos _servicioAccesoDatos;

        public Usuarios(ServicioAccesoDatos servicioAccesoDatos)
        {
            _servicioAccesoDatos = servicioAccesoDatos;
        }
        public async Task<GrupoUser?> ObtenerInformacionUsuario(string nombreUsuarioDA)
        {
            try
            {
                DataTable dtUsuario = _servicioAccesoDatos.TraerTablaParametros("Grupo_User", "IdGrupoUser, NameUser, IdGrupoTrabajo, IdRol, NombreUsua, Estado, PermisoConversacion", $"NameUser = '{nombreUsuarioDA}' AND Estado = 1");

                if (dtUsuario == null || dtUsuario.Rows.Count == 0)
                    return null;

                DataRow row = dtUsuario.Rows[0];

                GrupoUser informacionUsuario = new()
                {
                    IdGrupoUser = row.Field<int>("IdGrupoUser"),
                    NameUser = row.Field<string>("NameUser") ?? string.Empty,
                    IdGrupoTrabajo = row.Field<int>("IdGrupoTrabajo"),
                    IdRol = row.Field<int>("IdRol"),
                    NombreUsua = row.Field<string>("NombreUsua") ?? string.Empty,
                    Estado = row.Field<bool>("Estado"),
                    PermisoConversacion = row.Field<int>("PermisoConversacion")
                };

                return informacionUsuario;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
