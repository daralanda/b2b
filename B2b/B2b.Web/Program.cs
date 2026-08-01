using B2b.Dal.Context;
using B2b.Infrastructure.Service.MainService;
using B2b.Infrastructure.Service.PaymentService;
using B2b.Web.Middleware.JwtAuth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace B2b.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // DataProtection: App Pool recycle/restart olsa bile Cookie/Data Protection anahtarlarının kaybolmasını engeller.
        var keysFolder = Path.Combine(builder.Environment.ContentRootPath, "App_Data", "Keys");
        builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(keysFolder))
            .SetApplicationName("B2bApp");

        // DbContext
        builder.Services.AddDbContext<B2bDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")),
            ServiceLifetime.Transient);

        // CORS
        builder.Services.AddCors();
        builder.Services.AddEndpointsApiExplorer();

        // Memory Cache & Session Config
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromHours(10);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.Cookie.Name = "B2B_Session_Cookie";
        });

        // Authentication & Cookie Config
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromHours(10);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.Name = "B2B_Auth_Cookie";
            });

        builder.Services.AddHttpContextAccessor();

        // MVC / Controllers
        builder.Services.AddControllersWithViews()
            .AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                opt.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            });

        ServiceExtensions.ServiceRegisterAll(builder.Services, builder.Configuration);

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }

        app.UseForwardedHeaders();
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        // Global CORS
        app.UseCors(x => x
            .SetIsOriginAllowed(origin => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());

        // Custom SEO Rewrite Middleware
        app.Use(async (context, next) =>
        {
            var url = context.Request.Path.Value?.ToLower() ?? "";

            // API istekleri doğrudan geçsin (SEO rewrite işlemine takılmasın ve route bozulmasın)
            if (url.StartsWith("/api"))
            {
                await next();
                return;
            }

            if (!url.Contains(".js") && !url.Contains(".css") &&
                !url.Contains(".jpg") && !url.Contains(".ico") &&
                !url.Contains(".png") && !url.Contains(".xml") &&
                !url.Contains(".txt") && !url.Contains("/dashboard") &&
                !url.Contains(".map"))
            {
                var serviceMain = context.RequestServices.GetRequiredService<IMainService>();
                var main = serviceMain.GetSeo(url);

                if (main != null)
                {
                    context.Request.Path = $"/{main.ControllerName}/{main.ActionName}/{main.SeoId}";
                }
            }

            await next();
        });

        // Middleware Pipeline Sıralaması
        app.UseRouting();

        app.UseSession();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<JwtMiddleware>(); // Custom JWT Middleware

        // MapControllers: [ApiController] ve [Route("api/...")] Attribute Routing'in 404 vermeden çalışmasını sağlar.
        app.MapControllers();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}