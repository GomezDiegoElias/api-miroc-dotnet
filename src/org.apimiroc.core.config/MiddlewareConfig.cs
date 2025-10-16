using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
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
                            _ => 500
                        };

                        context.Response.StatusCode = statusCode;

                        var error = new ErrorDetails(statusCode, ex.Message, context.Request.Path, ex.StackTrace); // ex.StackTrace

                        var response = new StandardResponse<ErrorDetails>(false, "Ah ocurrido un error", null, error, statusCode);

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
