using Microsoft.AspNetCore.Mvc;
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
            var movementUpdated = await _service.Update(movementCaptured, code);
            var response = MovementMapper.ToResponseV2(movementUpdated);

            return Ok(new StandardResponse<MovementResponseV2>(
                true,
                "Movimiento actualizado exitosamente",
                response,
                null,
                200
            ));

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
