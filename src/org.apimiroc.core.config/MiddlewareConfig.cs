using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using org.apimiroc.core.entities.Exceptions;

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
                            _ => 500
                        };

                        context.Response.StatusCode = statusCode;

                        await context.Response.WriteAsJsonAsync(new
                        {
                            success = false,
                            message = ex.Message,
                            status = statusCode
                        });
                    }
                });
            });

            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();

            return app;
        }
    }
}
