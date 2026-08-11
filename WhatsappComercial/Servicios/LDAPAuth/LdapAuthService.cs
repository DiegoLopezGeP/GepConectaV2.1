using System.Diagnostics;
using System.DirectoryServices.Protocols;
using System.Net;
using WhatsappComercial.Interfaces.LDAPAuth;

namespace WhatsappComercial.Servicios.LDAPAuth
{
    public class LdapAuthService : ILdapAuthService
    {
        private readonly IConfiguration _configuration;

        public LdapAuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool ValidarCredenciales(string usuario, string password)
        {
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
                return false;

            string domainServer = _configuration["AppSettings:LdapServer"]!; // "192.168.1.50"
            string domainName = _configuration["AppSettings:DomainName"]!;     // "GEP"

            // Limpiar el usuario si enviaron "GEP\diego.lopez" o "diego.lopez@gep.dc"
            if (usuario.Contains("\\"))
            {
                usuario = usuario.Split('\\')[1];
            }
            else if (usuario.Contains("@"))
            {
                usuario = usuario.Split('@')[0];
            }

            // VARIACIÓN 1: Formato UPN (diego.lopez@GEP.DC) -> Es el más estándar en Active Directory moderno
            string upnUsuario = $"{usuario}@{domainName}.DC";

            // VARIACIÓN 2: Formato Downlevel/NetBIOS (GEP\diego.lopez)
            string netBiosUsuario = $"{domainName}\\{usuario}";

            // Intentar Variación 1 (UPN)
            if (ProbarBind(domainServer, upnUsuario, password, AuthType.Negotiate))
            {
                Debug.WriteLine(">>> [LDAP] Éxito con formato UPN");
                return true;
            }

            // Intentar Variación 2 (NetBIOS)
            if (ProbarBind(domainServer, netBiosUsuario, password, AuthType.Negotiate))
            {
                Debug.WriteLine(">>> [LDAP] Éxito con formato NetBIOS");
                return true;
            }

            // Intentar Variación 3 (Credencial estructurada NetworkCredential)
            try
            {
                LdapDirectoryIdentifier identifier = new LdapDirectoryIdentifier(domainServer, 389);
                NetworkCredential credential = new NetworkCredential(usuario, password, domainName);

                using (LdapConnection connection = new LdapConnection(identifier, credential))
                {
                    connection.AuthType = AuthType.Negotiate;
                    connection.SessionOptions.ProtocolVersion = 3;
                    connection.Bind();
                    Debug.WriteLine(">>> [LDAP] Éxito con NetworkCredential explícito");
                    return true;
                }
            }
            catch (LdapException ex)
            {
                Debug.WriteLine($">>> [LDAP ERROR FINAL]: {ex.Message} (Error Code: {ex.ErrorCode})");
                return false;
            }
        }

        private bool ProbarBind(string server, string username, string password, AuthType authType)
        {
            try
            {
                LdapDirectoryIdentifier identifier = new LdapDirectoryIdentifier(server, 389);
                NetworkCredential credential = new NetworkCredential(username, password);

                using (LdapConnection connection = new LdapConnection(identifier, credential))
                {
                    connection.AuthType = authType;
                    connection.SessionOptions.ProtocolVersion = 3;
                    connection.Bind();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
