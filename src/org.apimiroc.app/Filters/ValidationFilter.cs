using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FluentValidation;
using org.apimiroc.core.shared.Dto.General;

namespace org.apimiroc.app.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {

        // Inyección del proveedor de servicios para resolver validadores
        private readonly IServiceProvider _serviceProvider;

        public ValidationFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // Método que se ejecuta antes y después de la acción del controlador
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            // Itera sobre los parámetros de la acción
            foreach (var parameter in context.ActionDescriptor.Parameters)
            {
                var argument = context.ActionArguments[parameter.Name];
                if (argument == null) continue;

                var argumentType = argument.GetType();

                // busca un validador para el tipo del argumento
                var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);
                var validator = _serviceProvider.GetService(validatorType) as IValidator;

                // Si se encuentra un validador, ejecuta la validación
                if (validator != null)
                {
                    // Crea el contexto de validación
                    var validationContext = new ValidationContext<object>(argument);

                    // Ejecuta la validación
                    var validationResult = await validator.ValidateAsync(validationContext);

                    if (!validationResult.IsValid)
                    {
                        // Agrega los errores de validación al ModelState
                        foreach (var error in validationResult.Errors)
                        {
                            context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                        }
                    }
                }
            }

            // Verifica si el ModelState es válido
            if (!context.ModelState.IsValid)
            {

                // Construye la lista de errores
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors.Select(e =>
                        $"{x.Key}: {e.ErrorMessage}"))
                    .ToList();

                var errorDetails = new ErrorDetails(
                    400,
                    "Ah ocurrido un error",
                    context.HttpContext.Request.Path,
                    string.Join("; ", errors)
                );

                var response = new StandardResponse<object>(
                    false,
                    "Error de validación",
                    null,
                    errorDetails,
                    400
                );

                // Devuelve una respuesta de error con los detalles de validación
                context.Result = new BadRequestObjectResult(response);
                return;
            }

            // Si todo es válido, continúa con la ejecución de la acción
            await next();
        }
    }
}