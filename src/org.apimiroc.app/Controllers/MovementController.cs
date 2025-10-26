using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using org.apimiroc.app.Mappers;
using org.apimiroc.core.business.Services.Imp;
using org.apimiroc.core.shared.Dto.General;
using org.apimiroc.core.shared.Dto.Request;
using org.apimiroc.core.shared.Dto.Response;

namespace org.apimiroc.app.Controllers
{
    [ApiController]
    [Route("api/v1/movements")]
    public class MovementController : Controller
    {

        private readonly IMovementService _service;
        private readonly IValidator<MovementRequest> _movementValidation;

        public MovementController(IMovementService service, IValidator<MovementRequest> movementValidation)
        {
            _service = service;
            _movementValidation = movementValidation;
        }

        [HttpPost]
        public async Task<ActionResult<StandardResponse<MovementResponse>>> SaveMovemente([FromBody] MovementRequest request)
        {

            var validationResult = await _movementValidation.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var validationErrors = string.Join("; ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                var errors = new ErrorDetails(400, "Validacion fallida", HttpContext.Request.Path, validationErrors);
                return BadRequest(new StandardResponse<MovementResponse>(false, "Ah ocurrido un error", null, errors, 400));
            }

            var movementCaptured = MovementMapper.ToEntity(request);
            var movementSaved = await _service.Save(movementCaptured);
            var response = MovementMapper.ToResponse(movementSaved);

            return Created(string.Empty, new StandardResponse<MovementResponse>(
                true,
                "Movimiento creado exitosamente",
                response,
                null,
                201
            ));

        }

        [HttpGet]
        public async Task<ActionResult<StandardResponse<List<MovementResponse>>>> FindAllMovements()
        {

            var movements = await _service.FindAll();
            var response = movements.Select(m => MovementMapper.ToResponse(m)).ToList();

            return Ok(new StandardResponse<List<MovementResponse>>(
                true,
                "Movimientos obtenidos exitosamente",
                response,
                null,
                200
            ));

        }

    }
}
