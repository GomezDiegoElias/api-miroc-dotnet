using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace org.apimiroc.core.data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // 1️⃣ Leer la cadena de conexión desde variable de entorno
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_SQLSERVER");

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("No se encontró la cadena de conexión en la variable de entorno 'CONNECTION_SQLSERVER'.");

            // 2️⃣ Configurar DbContextOptions
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
