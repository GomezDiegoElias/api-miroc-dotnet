using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using org.apimiroc.core.data;

namespace org.apimiroc.core.config
{
    public static class DatabaseConfig
    {
        // Configura el DbContext según la configuración
        public static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var dbProvider = configuration.GetValue<string>("Database:Provider")?.Trim().ToLowerInvariant();

            if (string.IsNullOrEmpty(dbProvider))
                throw new InvalidOperationException("No se ha configurado un proveedor de base de datos válido");

            var connectionString = configuration.GetValue<string>($"CONNECTION_{dbProvider.ToUpper()}")
                ?? throw new InvalidOperationException($"Error al obtener la cadena de conexión para {dbProvider}");

            switch (dbProvider)
            {
                case "sqlserver":
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlServer(connectionString));
                    break;
                default:
                    throw new InvalidOperationException($"Proveedor de base de datos no soportado: {dbProvider}");
            }

            return services;
        }

        // Aplica migraciones de manera desacoplada
        public static void MigrateDatabase(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
        }
    }
}
