using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using org.apimiroc.app.Mappers;
using org.apimiroc.core.business.Services.Imp;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;
using org.apimiroc.core.shared.Dto.Request;
using org.apimiroc.core.shared.Dto.Response.Movements;
using org.apimiroc.core.shared.Dto.Response.Movements.V2;

namespace org.apimiroc.app.Controllers
{
    [ApiController]
    public class MovementController : Controller
    {

        private readonly IMovementService _service;

        public MovementController(IMovementService service)
        {
            _service = service;
        }

        // ░░░░░░░░░░░░░░░░░░░░░░░░░░░ ENDPOINTS VERSION 1 - RELACIONES CON CLAVES UNICAS ░░░░░░░░░░░░░░░░░░░░░░░░░░░

        [HttpGet("api/v1/movements")]
        public async Task<ActionResult<StandardResponse<PaginatedResponse<MovementResponse>>>> FindAll(
            [FromQuery] MovementFilter filters
        )
        {

            var movements = await _service.FindAll(filters);
            var response = movements.Items.Select(m => MovementMapper.ToResponse(m)).ToList();

            var paginatedResponse = new PaginatedResponse<MovementResponse>
            {
                Items = response,
                PageIndex = movements.PageIndex,
                PageSize = movements.PageSize,
                TotalItems = movements.TotalItems,
                TotalPages = movements.TotalPages
            };

            return Ok(new StandardResponse<PaginatedResponse<MovementResponse>>(
                true,
                "Movimientos obtenidos exitosamente",
                paginatedResponse,
                null,
                200
            ));

        }

        [HttpPost("api/v1/movements")]
        public async Task<ActionResult<StandardResponse<MovementResponse>>> Save([FromBody] MovementRequest request)
        {

            var movementToSave = await _service.Save(request);
            var response = MovementMapper.ToResponse(movementToSave);

            return Created(string.Empty, new StandardResponse<MovementResponse>(
                true,
                "Movimiento creado exitosamente",
                response,
                null,
                201
            ));

        }

        [HttpGet("api/v1/movements/{code:int}")]
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

        [HttpGet("api/v1/movements/{id}")]
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

        [HttpPut("api/v1/movements/{code:int}")]
        public async Task<ActionResult<StandardResponse<MovementResponse>>> UpdateByCode(
            int code,
            [FromBody] MovementRequest request
        )
        {

            var movementToUpdated = await _service.Update(request, code);
            var response = MovementMapper.ToResponse(movementToUpdated);

            return Ok(new StandardResponse<MovementResponse>(
                true,
                "Movimiento actualizado exitosamente",
                response,
                null,
                200
            ));

        }

        [HttpPatch("api/v1/movements/{code:int}")]
        public async Task<ActionResult<StandardResponse<MovementResponse>>> UpdatePartialByCode(
            int code,
            [FromBody] JsonPatchDocument<MovementRequest> patchDoc
        )
        {
            if (patchDoc == null)
            {
                var errors = new ErrorDetails(400, "Documento de parcheo nulo", HttpContext.Request.Path, "El documento de parcheo no puede ser nulo");
                return BadRequest(new StandardResponse<MovementResponse>(false, "Ah ocurrido un error", null, errors));
            }

            var existingMovement = await _service.FindByCode(code);

            var movementToPatch = MovementMapper.ToRequest(existingMovement);

            var modelState = new ModelStateDictionary();
            patchDoc.ApplyTo(movementToPatch, modelState);
            if (!modelState.IsValid)
            {
                var errors = string.Join("; ", modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                var errorDetails = new ErrorDetails(400, "Error de validacion", HttpContext.Request.Path, errors);
                return BadRequest(new StandardResponse<MovementResponse>(false, "Ah ocurrido un error", null, errorDetails));
            }

            var updatedMovement = await _service.UpdatePartial(movementToPatch, code);

            var response = MovementMapper.ToResponse(updatedMovement);

            return Ok(new StandardResponse<MovementResponse>(true, "Movimiento actualizado parcialmente con éxito", response));

        }

        [HttpDelete("api/v1/movements/{id}")]
        public async Task<ActionResult<StandardResponse<object>>> DeleteById(string id)
        {
            await _service.DeleteById(id);
            return Ok(new StandardResponse<object>(
                true,
                "Movimiento eliminado exitosamente",
                null,
                null,
                200
            ));
        }

        [HttpDelete("api/v1/movements/{code:int}")]
        public async Task<ActionResult<StandardResponse<object>>> DeleteByCode(int code)
        {
            await _service.DeleteByCode(code);
            return Ok(new StandardResponse<object>(
                true,
                "Movimiento eliminado exitosamente",
                null,
                null,
                200
            ));
        }

        // ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░ ENDPOINTS VERSION 2 - RELACIONES CON ID ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
        [HttpPost("api/v2/movements")]
        public async Task<ActionResult<StandardResponse<MovementResponseV2>>> SaveV2(
            [FromBody] MovementRequestV2 request
        )
        {

            var movementCaptured = MovementMapper.ToEntityV2(request);
            var movementSaved = await _service.SaveV2(movementCaptured);

            return Created(string.Empty, new StandardResponse<MovementResponseV2>(
                true,
                "Movimiento creado exitosamente",
                MovementMapper.ToResponseV2(movementSaved),
                null,
                201
            ));

        }

        [HttpGet("api/v2/movements")]
        public async Task<ActionResult<StandardResponse<PaginatedResponse<MovementResponseV2>>>> FindAllV2(
            [FromQuery] MovementFilter filters
        )
        {

            var movements = await _service.FindAllV2(filters);
            var response = movements.Items.Select(m => MovementMapper.ToResponseV2(m)).ToList();

            var paginatedResponse = new PaginatedResponse<MovementResponseV2>
            {
                Items = response,
                PageIndex = movements.PageIndex,
                PageSize = movements.PageSize,
                TotalItems = movements.TotalItems,
                TotalPages = movements.TotalPages
            };

            return Ok(new StandardResponse<PaginatedResponse<MovementResponseV2>>(
                true,
                "Movimientos obtenidos exitosamente",
                paginatedResponse,
                null,
                200
            ));

        }

        [HttpGet("api/v2/movements/{code:int}")]
        public async Task<ActionResult<StandardResponse<MovementResponseV2>>> GetByCodeV2(int code)
        {
            var movement = await _service.FindByCode(code);
            return Ok(new StandardResponse<MovementResponseV2>(
                true,
                "Movimiento obtenido exitosamente",
                MovementMapper.ToResponseV2(movement),
                null,
                200
            ));
        }

        [HttpGet("api/v2/movements/{id}")]
        public async Task<ActionResult<StandardResponse<MovementResponseV2>>> GetByIdV2(string id)
        {
            var movement = await _service.FindById(id);
            return Ok(new StandardResponse<MovementResponseV2>(
                true,
                "Movimiento obtenido exitosamente",
                MovementMapper.ToResponseV2(movement),
                null,
                200
            ));
        }

        // UPDATE
        [HttpPut("api/v2/movements/{code:int}")]
        public async Task<ActionResult<StandardResponse<MovementResponseV2>>> UpdateByCodeV2(
            int code,
            [FromBody] MovementRequestV2 request
        )
        {

            var movementCaptured = MovementMapper.ToEntityV2(request);
            var movementUpdated = await _service.UpdateV2(movementCaptured, code);
            var response = MovementMapper.ToResponseV2(movementUpdated);

            return Ok(new StandardResponse<MovementResponseV2>(
                true,
                "Movimiento actualizado exitosamente",
                response,
                null,
                200
            ));

        }

        [HttpPatch("api/v2/movements/{code:int}")]
        public async Task<ActionResult<StandardResponse<MovementResponseV2>>> UpdatePartialByCodeV2(
            int code,
            [FromBody] JsonPatchDocument<MovementRequestV2> patchDoc
        )
        {
            
            if (patchDoc == null)
            {
                var errors = new ErrorDetails(400, "Documento de parcheo nulo", HttpContext.Request.Path, "El documento de parcheo no puede ser nulo");
                return BadRequest(new StandardResponse<MovementResponseV2>(false, "Ah ocurrido un error", null, errors));
            }

            var existingMovement = await _service.FindByCode(code);

            var movementToPatch = MovementMapper.ToRequestV2(existingMovement);

            var modelState = new ModelStateDictionary();
            patchDoc.ApplyTo(movementToPatch, modelState);
            if (!modelState.IsValid)
            {
                var errors = string.Join("; ", modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                var errorDetails = new ErrorDetails(400, "Error de validacion", HttpContext.Request.Path, errors);
                return BadRequest(new StandardResponse<MovementResponseV2>(false, "Ah ocurrido un error", null, errorDetails));
            }

            var movementToUpdate = MovementMapper.ToEntityForPatchV2(movementToPatch, existingMovement);

            var updatedMovement = await _service.UpdatePartialV2(movementToUpdate, code);

            var response = MovementMapper.ToResponseV2(updatedMovement);

            return Ok(new StandardResponse<MovementResponseV2>(true, "Movimiento actualizado parcialmente con éxito", response));

        }


        [HttpDelete("api/v2/movements/{id}")]
        public async Task<ActionResult<StandardResponse<object>>> DeleteByIdV2(string id)
        {
            await _service.DeleteById(id);
            return Ok(new StandardResponse<object>(
                true,
                "Movimiento eliminado exitosamente",
                null,
                null,
                200
            ));
        }

        [HttpDelete("api/v2/movements/{code:int}")]
        public async Task<ActionResult<StandardResponse<object>>> DeleteByCodeV2(int code)
        {
            await _service.DeleteByCode(code);
            return Ok(new StandardResponse<object>(
                true,
                "Movimiento eliminado exitosamente",
                null,
                null,
                200
            ));
        }

    }
}
