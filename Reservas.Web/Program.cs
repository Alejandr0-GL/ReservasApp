using Reservas.Business;
using Reservas.Web.Services;

namespace Reservas.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Registrar servicios desde la capa Business (incluye DbContext)
            builder.Services.AddBusinessServices(builder.Configuration);

            builder.Services.AddControllersWithViews();

            builder.Services.AddMemoryCache();
            builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
            builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();

            //Cookies para manejar la sesión
            builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login"; // Ruta a la que redirige si no está autenticado
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Tiempo de expiración de sesión
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
