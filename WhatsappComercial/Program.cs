using System.Security.Claims;
using AccesoDatos;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.Extensions.FileProviders;
using Radzen;
using WhatsappComercial.Components;
using WhatsappComercial.Extensions;
using WhatsappComercial.Hubs;
using WhatsappComercial.Interfaces.Contactos;
using WhatsappComercial.Interfaces.Conversaciones;
using WhatsappComercial.Interfaces.Tickets;
using WhatsappComercial.Interfaces.Usuarios;
using WhatsappComercial.Modelos;
using WhatsappComercial.Servicios.AccesoADatos;
using WhatsappComercial.Servicios.AutenticacionUsuario;
using WhatsappComercial.Servicios.Contactos;
using WhatsappComercial.Servicios.Conversaciones;
using WhatsappComercial.Servicios.Tickets;
using WhatsappComercial.Servicios.Usuarios;

namespace WhatsappComercial
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
            .AddNegotiate();

            builder.Services.AddAuthorization(options =>
            {
                options.FallbackPolicy = options.DefaultPolicy;
            });
            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddRadzenComponents();

            builder.Services.AddHttpContextAccessor();
            builder.Services.Configure<ConfiguracionApp>(builder.Configuration.GetSection("AppSettings"));
  
            builder.Services.AddApplicationServices();

      

            builder.Services.AddRazorPages();
            builder.Services.AddHttpClient();
            builder.Services.AddServerSideBlazor();

            builder.Services.AddScoped<AccesoDatosSoapClient>(sp =>
                new AccesoDatosSoapClient(
                    AccesoDatosSoapClient.EndpointConfiguration.AccesoDatosSoap12));

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                var devUser = app.Configuration["DevImpersonationUser"];
                if (!string.IsNullOrEmpty(devUser))
                {
                    app.Use(async (context, next) =>
                    {
                        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, devUser),
                new Claim(ClaimTypes.NameIdentifier, devUser),
                new Claim(ClaimTypes.WindowsAccountName, devUser)
            };

                        var identity = new ClaimsIdentity(claims, "Windows");
                        var principal = new ClaimsPrincipal(identity);

                        context.User = principal;

                        await next();
                    });
                }
            }

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(@"C:\GepConecta\Archivos\Multimedia")),
                RequestPath = "/Multimedia"
            });
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.MapHub<GepConectaHub>("/GepConectaHub");

            app.Run();
        }
    }
}
