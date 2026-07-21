using WhatsappComercial.HostedService;
using WhatsappComercial.Interfaces.Cache;
using WhatsappComercial.Interfaces.Contactos;
using WhatsappComercial.Interfaces.Conversaciones;
using WhatsappComercial.Interfaces.Mensajes;
using WhatsappComercial.Interfaces.ProcesarMensajeEntrante;
using WhatsappComercial.Interfaces.Tickets;
using WhatsappComercial.Interfaces.Usuarios;
using WhatsappComercial.Interfaces.Utilidades;
using WhatsappComercial.Servicios.AccesoADatos;
using WhatsappComercial.Servicios.AutenticacionUsuario;
using WhatsappComercial.Servicios.Cache;
using WhatsappComercial.Servicios.ConfiguracionEstatica;
using WhatsappComercial.Servicios.Contactos;
using WhatsappComercial.Servicios.Conversaciones;
using WhatsappComercial.Servicios.Mensajes;
using WhatsappComercial.Servicios.ProcesarMensaje;
using WhatsappComercial.Servicios.Tickets;
using WhatsappComercial.Servicios.Usuarios;
using WhatsappComercial.Servicios.WebSocketService;
using WhatsappComercial.Utilidades;

namespace WhatsappComercial.Extensions
{
    public static class InyeccionDependencias
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //HosterService para iniciar la carga de conversacionesActivas
            services.AddHostedService<InicializadorCacheConversaciones>();

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
            //Servicio para construir plantillas de mensaje
            services.AddScoped<IConstruirMensajePlantilla, ConstruirMensajePlantillaService>();
            //Servicio para enviar mensajes
            services.AddScoped<IEnviarMensaje, EnviarMensajeService>();
            //ProcesarMensajes
            services.AddScoped<IProcesarMensajeEntrante, ProcesarMensajeService>();
            //Servicio WebSocket
            services.AddScoped<WebSocketServicio>();
            //Constructor de objeto Tarjeta Conversaciones
            services.AddScoped<IUtilidades, ConstruirObjetoTarjetaConversacion>();
            //Servicios Estaticos
            //Configuracion Aplicacion
            services.AddSingleton<ConfiguracionApp>();
            //Servicio para gestionar el cache de conversaciones
            services.AddSingleton<ICacheConversaciones, CacheConversacionesService>();

           

            return services;
        }
    }
}
