using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using org.apimiroc.core.business.Services;
using org.apimiroc.core.business.Services.Imp;
using org.apimiroc.core.data.Repositories;
using org.apimiroc.core.data.Repositories.Imp;

namespace org.apimiroc.core.config
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configura la base de datos
            services.AddDatabase(configuration);

            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.AddFixedWindowLimiter("fijo", opt =>
                {
                    opt.PermitLimit = 20;
                    opt.Window = TimeSpan.FromSeconds(5);
                });
            });

            // Repositorios
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();

            // Servicios
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
