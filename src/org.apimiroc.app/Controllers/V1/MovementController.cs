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
using org.apimiroc.core.shared.Dto.Response.Movements;

namespace org.apimiroc.app.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [Route("api/[controller]s")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]s")]
    [ApiVersion("1.0")]
    public class MovementController : ControllerBase
    {

        private readonly IMovementService _service;

        public MovementController(IMovementService service)
        {
            _service = service;
        }

        [Authorize(Policy = "CanREAD_Movement")]
        [HttpGet("summary")]
        public async Task<ActionResult<StandardResponse<TotalSummaryOfMovements>>> GetTotalSummary()
        {
            var summary =  await _service.getTotalSumarry();
            return Ok(new StandardResponse<TotalSummaryOfMovements>(
                true,
                "Resumen total de movimientos obtenido exitosamente",
                summary,
                null,
                200
            ));
        }

        [Authorize(Policy = "CanREAD_Movement")]
        [HttpGet]
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

        [Authorize(Policy = "CanCREATE_Movement")]
        [HttpPost]
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

        [Authorize(Policy = "CanREAD_Movement")]
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

        [Authorize(Policy = "CanREAD_Movement")]
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

        [Authorize(Policy = "CanUPDATE_Movement")]
        [HttpPut("{code:int}")]
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

        [Authorize(Policy = "CanUPDATE_Movement")]
        [HttpPatch("{code:int}")]
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

        [Authorize(Policy = "CanDELETE_Movement")]
        [HttpDelete("{id}")]
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

        [Authorize(Policy = "CanDELETE_Movement")]
        [HttpDelete("{code:int}")]
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

    }
}
