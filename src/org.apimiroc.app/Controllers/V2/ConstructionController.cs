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
using org.apimiroc.core.shared.Dto.Response;

namespace org.apimiroc.app.Controllers.V2
{

    [ApiController]
    [Route("api/[controller]")]
    [Route("api/[controller]s")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]s")]
    [ApiVersion("2.0")]
    public class ConstructionController : ControllerBase
    {

        private readonly IConstructionService _constructionService;

        public ConstructionController(IConstructionService constructionService)
        {
            _constructionService = constructionService;
        }

        [Authorize(Policy = "CanREAD_Construction")]
        [HttpGet]
        public async Task<ActionResult<StandardResponse<PaginatedResponse<ConstructionResponseV2>>>> FindAllConstructionsV2([FromQuery] ConstructionFilter filters)
        {

            var constructions = await _constructionService.FindAll(filters);
            var constructionResponse = constructions.Items.Select(c => ConstructionMapper.ToResponseV2(c)).ToList();

            var paginatedResponse = new PaginatedResponse<ConstructionResponseV2>
            {
                Items = constructionResponse,
                PageIndex = constructions.PageIndex,
                PageSize = constructions.PageSize,
                TotalItems = constructions.TotalItems,
                TotalPages = constructions.TotalPages
            };

            return Ok(new StandardResponse<PaginatedResponse<ConstructionResponseV2>>(
                Success: true,
                Message: "Construcciones obtenidas exitosamente",
                Data: paginatedResponse
            ));

        }

        [Authorize(Policy = "CanREAD_Construction")]
        [HttpGet("{name}")]
        public async Task<ActionResult<StandardResponse<ConstructionResponseV2>>> FindConstructionByNameV2(string name)
        {
            var construction = await _constructionService.FindByName(name);
            var response = ConstructionMapper.ToResponseV2(construction!);
            return Ok(new StandardResponse<ConstructionResponseV2>(true, "Construcción obtenida exitosamente", response));
        }

        [Authorize(Policy = "CanREAD_Construction")]
        [HttpGet("id/{id}")]
        public async Task<ActionResult<StandardResponse<ConstructionResponseV2>>> FindConstructionByIdV2(string id)
        {
            var construction = await _constructionService.FindById(id);
            var response = ConstructionMapper.ToResponseV2(construction!);
            return Ok(new StandardResponse<ConstructionResponseV2>(true, "Construcción obtenida exitosamente", response));
        }

        [Authorize(Policy = "CanCREATE_Construction")]
        [HttpPost]
        public async Task<ActionResult<StandardResponse<ConstructionResponseV2>>> SaveConstructionV2(
            [FromBody] ConstructionRequestV2 request
        )
        {

            var newConstruction = await _constructionService.SaveV2(request);
            var response = ConstructionMapper.ToResponseV2(newConstruction);

            return Created(string.Empty, new StandardResponse<ConstructionResponseV2>(
                true,
                "construcción creada exitosamente",
                response,
                null,
                201
            ));

        }

        [Authorize(Policy = "CanUPDATE_Construction")]
        [HttpPut("{name}")]
        public async Task<ActionResult<StandardResponse<ConstructionResponseV2>>> UpdateConstructionV2(
            [FromBody] ConstructionRequestV2 request,
            string name
        )
        {

            var existingConstruction = await _constructionService.FindByName(name);

            var constructionToUpdate = ConstructionMapper.ToEntityForUpdateV2(request, existingConstruction!);

            var updatedConstruction = await _constructionService.Update(constructionToUpdate, name);
            var response = ConstructionMapper.ToResponseV2(updatedConstruction);

            return Ok(new StandardResponse<ConstructionResponseV2>(true, "Construcción actualizada exitosamente", response));

        }

        [Authorize(Policy = "CanUPDATE_Construction")]
        [HttpPatch("{name}")]
        public async Task<ActionResult<StandardResponse<ConstructionResponseV2>>> PartiallyUpdateConstructionV2(
            string name,
            [FromBody] JsonPatchDocument<ConstructionRequestV2> patchDoc
        )
        {

            if (patchDoc == null)
            {
                var errors = new ErrorDetails(400, "Documento de parcheo nulo", HttpContext.Request.Path, "El documento de parcheo no puede ser nulo.");
                return BadRequest(new StandardResponse<ConstructionResponseV2>(false, "Ah ocurrido un error", null, errors));
            }

            var existingConstruction = await _constructionService.FindByName(name);

            var constructionToPatch = ConstructionMapper.ToRequestV2(existingConstruction!);

            var modelState = new ModelStateDictionary();
            patchDoc.ApplyTo(constructionToPatch, modelState);
            if (!modelState.IsValid)
            {
                var errors = string.Join("; ", modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                var errorDetails = new ErrorDetails(400, "Error de validacion", HttpContext.Request.Path, errors);
                return BadRequest(new StandardResponse<ConstructionResponseV2>(false, "Ah ocurrido un error", null, errorDetails));
            }

            var construction = ConstructionMapper.ToEntityForPatchV2(constructionToPatch, existingConstruction!);

            var updatedConstruction = await _constructionService.UpdatePartial(construction, name);

            var response = ConstructionMapper.ToResponseV2(updatedConstruction);

            return Ok(new StandardResponse<ConstructionResponseV2>(true, "Construcción actualizada parcialmente con éxito", response));

        }

        [Authorize(Policy = "CanDELETE_Construction")]
        [HttpDelete("permanent/{name}")]
        public async Task<ActionResult<StandardResponse<ConstructionResponseV2>>> DeleteConstructionV2(string name)
        {
            var existingConstruction = await _constructionService.FindByName(name);
            var deletedConstruction = await _constructionService.DeletePermanent(existingConstruction!.Name);
            var response = ConstructionMapper.ToResponseV2(deletedConstruction);
            return Ok(new StandardResponse<ConstructionResponseV2>(true, "Construcción eliminada exitosamente", response));
        }

        [Authorize(Policy = "CanDELETE_Construction")]
        [HttpDelete("{name}")]
        public async Task<ActionResult<StandardResponse<ConstructionRequestV2>>> DeleteConstructionLogicV2(string name)
        {
            var existingConstruction = await _constructionService.FindByName(name);
            var deletedConstruction = await _constructionService.DeleteLogic(existingConstruction!.Name);
            var response = ConstructionMapper.ToResponseV2(deletedConstruction);
            return Ok(new StandardResponse<ConstructionResponseV2>(true, "Construcción eliminada exitosamente", response));

        }

    }
}
