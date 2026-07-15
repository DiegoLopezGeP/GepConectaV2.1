using WhatsappComercial.Modelos;

namespace WhatsappComercial.Interfaces.Usuarios
{
    public interface IUsuarios
    {
        Task<GrupoUser> ObtenerInformacionUsuario(string nombreUsuarioDA);
    }
}
