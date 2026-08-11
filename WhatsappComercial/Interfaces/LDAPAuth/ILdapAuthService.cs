namespace WhatsappComercial.Interfaces.LDAPAuth
{
    public interface ILdapAuthService
    {
        bool ValidarCredenciales(string usuario, string password);
    }
}
