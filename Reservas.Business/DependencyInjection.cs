using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reservas.DataAccess;

namespace Reservas.Business
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Registrar DbContext (Business referencia DataAccess internamente)
            services.AddDbContext<FondoDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Registrar servicios de negocio
            services.AddScoped<ReservaService>();

            return services;
        }
    }
}