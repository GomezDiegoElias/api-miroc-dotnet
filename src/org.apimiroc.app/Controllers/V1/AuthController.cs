using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using org.apimiroc.core.business.Services.Imp;
using org.apimiroc.core.shared.Dto.General;
using org.apimiroc.core.shared.Dto.Request;
using org.apimiroc.core.shared.Dto.Response;

namespace org.apimiroc.app.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [Route("api/[controller]s")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]s")]
    [ApiVersion("1.0")]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;
        private readonly IValidator<RegisterRequest> _registerValidator;
        private readonly IValidator<LoginRequest> _loginValidator;
        private ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            IValidator<RegisterRequest> registerRequest,
            IValidator<LoginRequest> loginRequest,
            ILogger<AuthController> logger
        )
        {
            _authService = authService;
            _registerValidator = registerRequest;
            _loginValidator = loginRequest;
            _logger = logger;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<StandardResponse<AuthResponse>>> Register([FromBody] RegisterRequest request)
        {

            var validationResult = await _registerValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var validationErrors = string.Join(", ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                var errors = new ErrorDetails(400, "Validación fallida", HttpContext.Request.Path, validationErrors);
                return new StandardResponse<AuthResponse>(false, "Ah ocurrido un error", null, errors, 400);
            }

            var registerResponse = await _authService.Register(request);
            
            var response = new StandardResponse<AuthResponse>(true, "Registro exitosamente", registerResponse);

            return Created(string.Empty, response);
        
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<StandardResponse<AuthResponse>>> Login([FromBody] LoginRequest request)
        {

            _logger.LogInformation("Intento de login para email: {Email}", request.Email);

            var validationResult = await _loginValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var validationErrors = string.Join("; ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                _logger.LogWarning("Validacion fallida en login: {Errors}", validationErrors);

                var errors = new ErrorDetails(400, "Validacion fallida", HttpContext.Request.Path, validationErrors);
                return new StandardResponse<AuthResponse>(false, "Ah ocurrido un error", null, errors, 400);
            }

            var loginResponse = await _authService.Login(request);
            _logger.LogInformation("Usuario logeado exitosamente: {Email}", request.Email);

            var response = new StandardResponse<AuthResponse>(true, "Inicio de sesion exitoso", loginResponse);

            return Ok(response);

        }

    }
}
