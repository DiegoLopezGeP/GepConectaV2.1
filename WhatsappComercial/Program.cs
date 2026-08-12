using System.Security.Claims;
using GepConecta.WhatsAppCloud.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.FileProviders;
using Radzen;
using Serilog;
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

            // ==========================================
            // CONFIGURACIÓN DE LOGGING SERILOG (TXT)
            // ==========================================
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(
                    path: Path.Combine(AppContext.BaseDirectory, "Logs", "log-.txt"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();

            builder.Host.UseSerilog();

            try
            {
                Log.Information("Iniciando aplicación WhatsApp Comercial...");

                // 1. Agregar Controladores
                builder.Services.AddControllers();

                // 2. Configuración de Autenticación mediante Cookies
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
                    // Lax permite persistencia de cookie tanto en navegadores de escritorio como en Safari iOS
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.LoginPath = "/login";
                    options.AccessDeniedPath = "/login";
                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                    options.SlidingExpiration = true;
                });

                // Servicios de la App
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

                // Configuración del Hub de SignalR / Blazor
                builder.Services.AddServerSideBlazor()
                    .AddHubOptions(options =>
                    {
                        options.MaximumReceiveMessageSize = 1024 * 1024; // 1 MB
                    });

                builder.Services.Configure<Microsoft.AspNetCore.Components.Server.CircuitOptions>(options =>
                {
                    options.DetailedErrors = true;
                });

                var app = builder.Build();

                // ==========================================
                // MIDDLEWARES DE INFRAESTRUCTURA
                // ==========================================

                // Encabezados reenviados (HAProxy / Proxies / IIS)
                var forwardedHeadersOptions = new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor
                                      | ForwardedHeaders.XForwardedProto
                                      | ForwardedHeaders.XForwardedHost
                };
                forwardedHeadersOptions.KnownIPNetworks.Clear();
                forwardedHeadersOptions.KnownProxies.Clear();
                app.UseForwardedHeaders(forwardedHeadersOptions);

                // Entorno de Desarrollo (DevImpersonationUser)
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

                // Subruta base en IIS
                app.UsePathBase("/WhatsappCorporativo");

                // Archivos Estáticos
                app.UseStaticFiles();

                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(@"C:\GepConecta\Archivos\Multimedia"),
                    RequestPath = "/Multimedia"
                });

                app.UseRouting();

                // Middleware para compatibilidad estricta de Cookies en móviles (Safari/iOS)
                app.UseCookiePolicy(new CookiePolicyOptions
                {
                    MinimumSameSitePolicy = SameSiteMode.Lax
                });

                app.UseAuthentication();
                app.UseAuthorization();

                app.UseAntiforgery();

                // Mapeo de Endpoints
                app.MapStaticAssets();
                app.MapControllers();

                app.MapRazorComponents<App>()
                    .AddInteractiveServerRenderMode();

                app.MapHub<GepConectaHub>("/GepConectaHub")
                    .DisableAntiforgery();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "La aplicación no pudo iniciar correctamente.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}