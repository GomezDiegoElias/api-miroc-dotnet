using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using org.apimiroc.app.Mappers;
using org.apimiroc.core.business.Services.Imp;
using org.apimiroc.core.entities.Exceptions;
using org.apimiroc.core.shared.Dto.General;
using org.apimiroc.core.shared.Dto.Request;
using org.apimiroc.core.shared.Dto.Response.Movements;

namespace org.apimiroc.app.Controllers
{
    [ApiController]
    [Route("api/v1/movements")]
    public class MovementController : Controller
    {

        private readonly IMovementService _service;
        private readonly IConceptService _serviceConcept;
        private readonly IValidator<MovementRequest> _movementValidation;

        public MovementController(IMovementService service, IValidator<MovementRequest> movementValidation, IConceptService serviceConcept)
        {
            _service = service;
            _movementValidation = movementValidation;
            _serviceConcept = serviceConcept;
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

            var conceptExists = await _serviceConcept.FindById(request.ConceptId)
                ?? throw new ConceptNotFoundException(request.ConceptId);

            var movementCaptured = MovementMapper.ToEntity(request);
            var movementSaved = await _service.Save(movementCaptured);
            //var response = MovementMapper.ToResponse(movementSaved);

            return Created(string.Empty, new StandardResponse<MovementResponse>(
                true,
                "Movimiento creado exitosamente",
                null, // response
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

        [HttpGet("{code:int}")]
        public async Task<ActionResult<StandardResponse<MovementResponse>>> GetByCode(int code)
        {
            var movement = await _service.FindByCode(code);
            return Ok(new StandardResponse<MovementResponse>(
                true,
                "Movimiento obtenido exitosamente",
                MovementMapper.ToResponse(movement),
                null,
                200
            ));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StandardResponse<MovementResponse>>> GetById(string id)
        {
            var movement = await _service.FindById(id);
            return Ok(new StandardResponse<MovementResponse>(
                true,
                "Movimiento obtenido exitosamente",
                MovementMapper.ToResponse(movement),
                null,
                200
            ));
        }

        [HttpPut("{code:int}")]
        public async Task<ActionResult<StandardResponse<MovementResponse>>> UpdateMovementByCode(int code, [FromBody] MovementRequest request)
        {

            var movementCaptured = MovementMapper.ToEntity(request);
            var movementUpdated = await _service.Update(movementCaptured, code);
            var response = MovementMapper.ToResponse(movementUpdated);

            return Ok(new StandardResponse<MovementResponse>(
                true,
                "Movimiento actualizado exitosamente",
                response,
                null,
                200
            ));

        }

    }
}
