using WhatsappComercial.Interfaces.Contactos;
using WhatsappComercial.Interfaces.Conversaciones;
using WhatsappComercial.Interfaces.Tickets;
using WhatsappComercial.Interfaces.Usuarios;
using WhatsappComercial.Servicios.AccesoADatos;
using WhatsappComercial.Servicios.AutenticacionUsuario;
using WhatsappComercial.Servicios.Contactos;
using WhatsappComercial.Servicios.Conversaciones;
using WhatsappComercial.Servicios.Tickets;
using WhatsappComercial.Servicios.Usuarios;

namespace WhatsappComercial.Extensions
{
    public static class InyeccionDependencias
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //Servicio Web Acceso Datos
            services.AddScoped<ServicioAccesoDatos>();
            //Autenticacion Usuario
            services.AddScoped<ObtenerNombreUsuario>();
            //Informacion Usuario Conectado
            services.AddScoped<IUsuarios, Usuarios>();
            //Contactos
            services.AddScoped<IBusquedaContactos, BusquedaContactos>();
            services.AddScoped<IGuardarContacto, GuardarContacto>();
            //Conversaciones
            services.AddScoped<ICrearConversacion, CrearConversacionService>();
            services.AddScoped<IObtenerConversaciones, ObtenerConversacionesService>();
            //Tickets
            services.AddScoped<IAsignarTicket, AsignarTicketService>();
            services.AddScoped<ICrearTicket, CrearTicketService>();

            return services;
        }
    }
}
