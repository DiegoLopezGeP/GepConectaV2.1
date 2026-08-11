using System.Security.Claims;
using GepConecta.WhatsAppCloud.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.HttpOverrides; // <-- Agregar esta directiva
using Microsoft.Extensions.FileProviders;
using Radzen;
using WhatsappComercial.Components;
using WhatsappComercial.Extensions;
using WhatsappComercial.Hubs;
using WhatsappComercial.Modelos;

namespace WhatsappComercial
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Agregar el servicio de Controladores
            builder.Services.AddControllers();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = "WhatsappComercialSession";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax; // Permite guardar la cookie en redirecciones locales
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.LoginPath = "/login";
                options.AccessDeniedPath = "/login";
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
            });

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddRadzenComponents();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
            builder.Services.Configure<ConfiguracionApp>(builder.Configuration.GetSection("AppSettings"));

            builder.Services.AddApplicationServices();

            builder.Services.AddRazorPages();
            builder.Services.AddHttpClient();

            builder.Services.AddHttpClient<IWhatsAppCloudClient, WhatsAppCloudClient>(client =>
            {
                var config = builder.Configuration.GetSection("WhatsAppSettings");
                string accessToken = config["AccessToken"]!;
                string phoneNumberId = config["PhoneNumberId"]!;

                return new WhatsAppCloudClient(client, accessToken, phoneNumberId);
            });

            builder.Services.AddServerSideBlazor()
            .AddHubOptions(options =>
            {
                options.MaximumReceiveMessageSize = 1024 * 1024; // aprovecho para subirlo por el tema de audio Base64 que ya tienes documentado
            });

            builder.Services.Configure<Microsoft.AspNetCore.Components.Server.CircuitOptions>(options =>
            {
                options.DetailedErrors = true;
            }
            );

            var app = builder.Build();

            // 1. RECOMENDADO: Capturar encabezados reenviados por HAProxy
            var forwardedHeadersOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor
                                  | ForwardedHeaders.XForwardedProto
                                  | ForwardedHeaders.XForwardedHost
            };
            forwardedHeadersOptions.KnownIPNetworks.Clear();
            forwardedHeadersOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(forwardedHeadersOptions);

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

            // 2. Establecer la subruta base antes del enrutamiento y archivos estáticos
            app.UsePathBase("/WhatsappCorporativo");

            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(@"C:\GepConecta\Archivos\Multimedia")),
                RequestPath = "/Multimedia"
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapControllers();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.MapHub<GepConectaHub>("/GepConectaHub")
                .DisableAntiforgery();

            app.Run();
        }
    }
}