using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using org.apimiroc.core.entities.Exceptions;
using org.apimiroc.core.shared.Dto.General;

namespace org.apimiroc.core.config
{
    public static class MiddlewareConfig
    {
        public static IApplicationBuilder UseGlobalMiddlewares(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (contextFeature != null)
                    {
                        var ex = contextFeature.Error;

                        // uso de log generico
                        var logger = context.RequestServices.GetRequiredService<ILoggerFactory>()
                            .CreateLogger("GlobalExceptionHandler");

                        int statusCode = ex switch
                        {
                            KeyNotFoundException => 404,
                            UnauthorizedAccessException => 401,
                            ApplicationException => 400,
                            UserNotFoundException => 404,
                            RoleNotFoundException => 404,
                            ClientNotFoundException => 404,
                            EmployeeNotFoundException => 404,
                            ProviderNotFoundException => 404,
                            ConstructionNotFoundException => 404,
                            ConceptNotFoundException => 404,
                            MovementNotFoundException => 404,
                            _ => 500
                        };

                        // Logueo segun la severidad
                        switch(statusCode)
                        {
                            case >= 500:
                                logger.LogError(ex, "Error interno en {Path}: {Message}", context.Request.Path, ex.Message);
                                break;
                            case 404:
                                logger.LogWarning("Recurso no encontrado en {Path}: {Message}", context.Request.Path, ex.Message);
                                break;
                            default:
                                logger.LogWarning(ex, "Error en {Path}: {Message}", context.Request.Path, ex.Message);
                                break;
                        }

                        context.Response.StatusCode = statusCode;

                        // No incluye el StackTrace en producción
                        var isDevelopment = context.RequestServices
                            .GetRequiredService<IWebHostEnvironment>()
                            .IsDevelopment();

                        var error = new ErrorDetails(
                            statusCode,
                            ex.Message,
                            context.Request.Path,
                            isDevelopment ? ex.StackTrace : null
                        );

                        var response = new StandardResponse<ErrorDetails>(
                            false,
                            "Ha ocurrido un error",
                            null,
                            error,
                            statusCode
                        );

                        await context.Response.WriteAsJsonAsync(response);
                    }
                });
            });

            // Capturar error de mal URL
            app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == StatusCodes.Status404NotFound &&
                    !context.Response.HasStarted)
                {
                    // uso de log generico
                    var logger = context.RequestServices.GetRequiredService<ILoggerFactory>()
                        .CreateLogger("GlobalExceptionHandler");

                    logger.LogWarning("Endpoint no encontrado: {Method} {Path}",
                        context.Request.Method, context.Request.Path);

                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new
                    {
                        StatusCode = 404,
                        Message = $"La url {context.Request.Path} no existe."
                    });
                }
            });

            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();

            return app;
        }
    }
}