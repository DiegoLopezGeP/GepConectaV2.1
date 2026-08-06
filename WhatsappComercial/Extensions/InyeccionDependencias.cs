using AccesoDatos;
using WhatsappComercial.HostedService;
using WhatsappComercial.Interfaces.Cache;
using WhatsappComercial.Interfaces.Contactos;
using WhatsappComercial.Interfaces.Conversaciones;
using WhatsappComercial.Interfaces.GestionArchivos;
using WhatsappComercial.Interfaces.Mensajes;
using WhatsappComercial.Interfaces.NotificarUI;
using WhatsappComercial.Interfaces.Plantillas;
using WhatsappComercial.Interfaces.ProcesarMensajeEntrante;
using WhatsappComercial.Interfaces.Tickets;
using WhatsappComercial.Interfaces.Usuarios;
using WhatsappComercial.Interfaces.Utilidades;
using WhatsappComercial.Servicios.AccesoADatos;
using WhatsappComercial.Servicios.AutenticacionUsuario;
using WhatsappComercial.Servicios.BackgroundServiceCache;
using WhatsappComercial.Servicios.Cache;
using WhatsappComercial.Servicios.ConfiguracionEstatica;
using WhatsappComercial.Servicios.Contactos;
using WhatsappComercial.Servicios.Conversaciones;
using WhatsappComercial.Servicios.GestionArchivos;
using WhatsappComercial.Servicios.Mensajes;
using WhatsappComercial.Servicios.NotificarUI;
using WhatsappComercial.Servicios.Plantillas;
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

            //HosterService para iniciar Servicio WebSocket
            services.AddHostedService<WebSocketServicio>();
            //HosterService para iniciar la carga de conversacionesActivas
            services.AddHostedService<InicializadorCacheConversaciones>();
            //HosterService para iniciar el worker de persistencia de estados de mensajes
            services.AddHostedService<WorkerPersistenciaEstados>();


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
            // Servicio para Modificar el estado de un mensaje en la base de datos
            services.AddScoped<IModificarEstadoMensaje, ModificarEstadoMensaje>();
            //Servicio para Finalizar una conversacion
            services.AddScoped<IFinalizarConversacion, FinalizarConversacion>();
            //Servicio para Obtener las plantillas activas por grupo de trabajo
            services.AddScoped<IObtenerPlantillas, ObtenerPlantillas>();
            //Servicio para crear helpers que puedan usarse en contactos
            services.AddScoped<IHelperContacto, HelperContacto>();
            //Servicio para consultar contactos en la base de datos de afiliaciones
            services.AddScoped<IBusquedaContactoAfiliaciones, BusquedaContactoAfiliaciones>();


            //Servicio para representar el formulario de registro de contacto
            services.AddScoped<IFormularioContactoService, FormularioContactoService>();

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
            services.AddSingleton<IColaEstadoMensajes, ColaEstadoMensajesService>();




            return services;
        }
    }
}
