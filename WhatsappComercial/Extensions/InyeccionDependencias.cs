using AccesoDatos;
using WhatsappComercial.HostedService;
using WhatsappComercial.Interfaces.Cache;
using WhatsappComercial.Interfaces.Contactos;
using WhatsappComercial.Interfaces.Conversaciones;
using WhatsappComercial.Interfaces.GestionArchivos;
using WhatsappComercial.Interfaces.Mensajes;
using WhatsappComercial.Interfaces.NotificarUI;
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
using WhatsappComercial.Servicios.GestionArchivos;
using WhatsappComercial.Servicios.Mensajes;
using WhatsappComercial.Servicios.NotificarUI;
using WhatsappComercial.Servicios.ProcesarMensaje;
using WhatsappComercial.Servicios.SystemServicio;
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

            // Registrar el servicio de memoria en el contenedor DI
            services.AddMemoryCache();

            //osterService para iniciar Servicio WebSocket
            services.AddHostedService<WebSocketServicio>();
            //HosterService para iniciar la carga de conversacionesActivas
            services.AddHostedService<InicializadorCacheConversaciones>();

            //Servicio Web Acceso Datos
            services.AddScoped<ServicioAccesoDatos>();
            services.AddScoped<AccesoDatosSoapClient>(sp => new AccesoDatosSoapClient(AccesoDatosSoapClient.EndpointConfiguration.AccesoDatosSoap12));
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
            //Servicio de Gestion de Archivos
            services.AddScoped<IGestionArchivos, GestionArchivoService>();
            //Constructor de objeto Tarjeta Conversaciones
            services.AddScoped<IUtilidades, ConstruirObjetoTarjetaConversacion>();
            //Servicio para gestionar los mensajes desde DB
            services.AddScoped<IObtenerMensajesConversacion, ObtenerMensajesConversacionService>();
            //Servicio para procesar el mensaje entrante proveniente del webhook
            services.AddScoped<IProcesarMensajeEntrante, ProcesarMensajeService>();
            //Servicio para procesar los mensajes de tipo texto entrantes
            services.AddScoped<IManejadorMensajeTexto, ManejadorMensajeTexto>();
            //Servicio para procesar los mensajes de tipo Multimedia
            services.AddScoped<IManejadorMensajeArchivo, ManejadorMensajeArchivo>();
            //Servicio para procesar los estados de lectura enviados por el webhook
            services.AddScoped<IManejadorEstadoLectura, ManejadorEstadoLectura>();
            //Servicio para validar Existencia de una conversacion
            services.AddScoped<IValidarExistenciaConversacion, ValidarExistenciaConversacionService>();
            // Servicio systemServicio
            services.AddScoped<SystemService>();

            //Servicios Estaticos
            //Configuracion Aplicacion
            services.AddSingleton<ConfiguracionEstaticaApp>();
            //Servicio para gestionar el cache de conversaciones
            services.AddSingleton<ICacheConversaciones, CacheConversacionesService>();
            //Servicio para validar mensajes Duplicados
            services.AddSingleton<IServicioValidadorDuplicados, ServicioValidadorDuplicados>();
            //Servicio para gestionar el cache de mensajes
            services.AddSingleton<ICacheMensajes, CacheMensajes>();
            //Servicio para notificar a la UI
            services.AddSingleton<INotificarUI, NotificarUIService>();




            return services;
        }
    }
}
