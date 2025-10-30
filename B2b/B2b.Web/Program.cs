using B2b.Dal.Context;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using B2b.Web.Middleware.JwtAuth;
using B2b.Infrastructure.Service.MainService;

namespace B2b.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        //builder.Services.AddDbContext<B2bDbContext>(options =>
        //{
        //    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"));
        //});
        builder.Services.AddDbContext<B2bDbContext>(options =>
                 options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")),
      ServiceLifetime.Transient);
        builder.Services.AddCors();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(60);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddControllersWithViews()
                .AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore)
                .AddNewtonsoftJson(opt => opt.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver())
                ;
        ServiceExtensions.ServiceRegisterAll(builder.Services, builder.Configuration);
        var app = builder.Build();
        if (!app.Environment.IsDevelopment())
        {
            //app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseCors(x => x
                .SetIsOriginAllowed(origin => true)
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials());
        app.UseMiddleware<JwtMiddleware>();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.UseSession();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.Use(async (context, next) =>
        {
            var _serviceMain = builder.Services.BuildServiceProvider().GetService<IMainService>();
            var url = context.Request.Path.ToString().ToLower();
            if (!url.Contains("/api/") && !url.Contains(".js") && !url.Contains(".css") && !url.Contains(".jpg") && !url.Contains(".ico") && !url.Contains(".png") && !url.Contains(".xml") && !url.Contains(".txt") && !url.ToLower().Contains("/dashboard") && !url.ToLower().Contains(".map"))
            {
                var main = _serviceMain.GetSeo(url);
                context.Request.Path = "/" + main.ControllerName + "/" + main.ActionName + "/" + main.SeoId;
            }
            else
            {
                context.Request.Path = url;
            }


            await next();
        });
        app.UseCors(x => x.SetIsOriginAllowed(origin => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials());
        app.UseForwardedHeaders();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.Run();
    }
}
