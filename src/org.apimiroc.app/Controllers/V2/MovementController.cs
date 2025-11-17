using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using org.apimiroc.app.Mappers;
using org.apimiroc.core.business.Services.Imp;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;
using org.apimiroc.core.shared.Dto.Request;
using org.apimiroc.core.shared.Dto.Response.Movements.V2;

namespace org.apimiroc.app.Controllers.V2
{

    [ApiController]
    [Route("api/[controller]")]
    [Route("api/[controller]s")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]s")]
    [ApiVersion("2.0")]
    public class MovementController : ControllerBase
    {

        private readonly IMovementService _service;
        public MovementController(IMovementService service)
        {
            _service = service;
        }

        [Authorize(Policy = "CanREAD_Movement")]
        [HttpGet]
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

        [Authorize(Policy = "CanCREATE_Movement")]
        [HttpPost]
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

        [Authorize(Policy = "CanREAD_Movement")]
        [HttpGet("{code:int}")]
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

        [Authorize(Policy = "CanREAD_Movement")]
        [HttpGet("{id}")]
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
        [Authorize(Policy = "CanUPDATE_Movement")]
        [HttpPut("{code:int}")]
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

        [Authorize(Policy = "CanUPDATE_Movement")]
        [HttpPatch("{code:int}")]
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

        [Authorize(Policy = "CanDELETE_Movement")]
        [HttpDelete("{id}")]
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

        [Authorize(Policy = "CanDELETE_Movement")]
        [HttpDelete("{code:int}")]
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
