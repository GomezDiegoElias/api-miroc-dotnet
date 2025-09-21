using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using org.apimiroc.core.data;
using org.apimiroc.core.data.Sedders;

namespace org.apimiroc.core.config
{
    public static class DbSeederConfig
    {
        public static async Task SeedAsync(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await DbSedder.SeedRolesAndPermissions(db);
            await DbSedder.SeedUserWhitDiferentRoles(db);
        }
    }

}
