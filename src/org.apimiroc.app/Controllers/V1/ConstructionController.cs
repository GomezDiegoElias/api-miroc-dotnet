using Asp.Versioning;
using FluentValidation;
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

namespace org.apimiroc.app.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [Route("api/[controller]s")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]s")]
    [ApiVersion("1.0")]
    public class ConstructionController : ControllerBase
    {

        private readonly IConstructionService _constructionService;
        private readonly IClientService _clientService;

        public ConstructionController(IConstructionService constructionService, IClientService clientService)
        {
            _constructionService = constructionService;
            _clientService = clientService;
        }

        [Authorize(Policy = "CanREAD_Construction")]
        [HttpGet]
        public async Task<ActionResult<StandardResponse<PaginatedResponse<ConstructionResponse>>>> FindAll([FromQuery] ConstructionFilter filters)
        {

            var constructions = await _constructionService.FindAll(filters);
            var constructionResponse = constructions.Items.Select(c => ConstructionMapper.ToResponse(c)).ToList();

            var paginatedResponse = new PaginatedResponse<ConstructionResponse>
            {
                Items = constructionResponse,
                PageIndex = constructions.PageIndex,
                PageSize = constructions.PageSize,
                TotalItems = constructions.TotalItems,
                TotalPages = constructions.TotalPages
            };

            return Ok(new StandardResponse<PaginatedResponse<ConstructionResponse>>(
                Success: true,
                Message: "Construcciones obtenidas exitosamente",
                Data: paginatedResponse
            ));

        }

        [Authorize(Policy = "CanCREATE_Construction")]
        [HttpPost]
        public async Task<ActionResult<StandardResponse<ConstructionResponse>>> SaveConstruction(
            [FromBody] ConstructionRequest request
        )
        {

            var constructionToSave = await _constructionService.Save(request);
            var response = ConstructionMapper.ToResponse(constructionToSave);

            return Created(string.Empty, new StandardResponse<ConstructionResponse>(
                true, 
                "Construcción creada exitosamente", 
                response, 
                null, 
                200
            ));

        }

        [Authorize(Policy = "CanREAD_Construction")]
        [HttpGet("{name}")]
        public async Task<ActionResult<StandardResponse<ConstructionResponse>>> FindByName(string name)
        {
            var construction = await _constructionService.FindByName(name);
            var response = ConstructionMapper.ToResponse(construction!);
            return Ok(new StandardResponse<ConstructionResponse>(true, "Construcción obtenida exitosamente", response));
        }

        [Authorize(Policy = "CanREAD_Construction")]
        [HttpGet("id/{id}")]
        public async Task<ActionResult<StandardResponse<ConstructionResponse>>> FindConstructionById(string id)
        {
            var construction = await _constructionService.FindById(id);
            var response = ConstructionMapper.ToResponse(construction!);
            return Ok(new StandardResponse<ConstructionResponse>(true, "Construcción obtenida exitosamente", response));
        }

        [Authorize(Policy = "CanUPDATE_Construction")]
        [HttpPut("{name}")]
        public async Task<ActionResult<StandardResponse<ConstructionResponse>>> UpdateConstruction(
            [FromBody] ConstructionRequest request,
            string name
        )
        {

            var existingConstruction = await _constructionService.FindByName(name);

            var client = await _clientService.FindByDni(request.ClientDni);

            var constructionToUpdate = ConstructionMapper.ToEntityForUpdate(request, existingConstruction!, client!.Id);

            var updatedConstruction = await _constructionService.Update(constructionToUpdate, name);
            var response = ConstructionMapper.ToResponse(updatedConstruction);

            return Ok(new StandardResponse<ConstructionResponse>(true, "Construcción actualizada exitosamente", response));

        }

        [Authorize(Policy = "CanUPDATE_Construction")]
        [HttpPatch("{name}")]
        public async Task<ActionResult<StandardResponse<ConstructionResponse>>> PartiallyUpdateConstruction(
            string name,
            [FromBody] JsonPatchDocument<ConstructionRequest> patchDoc
        )
        {

            if (patchDoc == null)
            {
                var errors = new ErrorDetails(400, "Documento de parcheo nulo", HttpContext.Request.Path, "El documento de parcheo no puede ser nulo.");
                return BadRequest(new StandardResponse<ConstructionResponse>(false, "Ah ocurrido un error", null, errors));
            }

            var existingConstruction = await _constructionService.FindByName(name);

            var constructionToPatch = ConstructionMapper.ToRequest(existingConstruction!);

            var modelState = new ModelStateDictionary();
            patchDoc.ApplyTo(constructionToPatch, modelState);
            if (!modelState.IsValid)
            {
                var errors = string.Join("; ", modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                var errorDetails = new ErrorDetails(400, "Error de validacion", HttpContext.Request.Path, errors);
                return BadRequest(new StandardResponse<ConstructionResponse>(false, "Ah ocurrido un error", null, errorDetails));
            }

            var client = await _clientService.FindByDni(constructionToPatch.ClientDni);

            var construction = ConstructionMapper.ToEntityForPatch(constructionToPatch, existingConstruction!, client!.Id);

            var updatedConstruction = await _constructionService.UpdatePartial(construction, name);

            var response = ConstructionMapper.ToResponse(updatedConstruction);

            return Ok(new StandardResponse<ConstructionResponse>(true, "Construcción actualizada parcialmente con éxito", response));

        }

        [Authorize(Policy = "CanDELETE_Construction")]
        [HttpDelete("permanent/{name}")]
        public async Task<ActionResult<StandardResponse<ConstructionResponse>>> DeleteConstruction(string name)
        {
            var existingConstruction = await _constructionService.FindByName(name);
            var deletedConstruction = await _constructionService.DeletePermanent(existingConstruction!.Name);
            var response = ConstructionMapper.ToResponse(deletedConstruction);
            return Ok(new StandardResponse<ConstructionResponse>(true, "Construcción eliminada exitosamente", response));
        }

        [Authorize(Policy = "CanDELETE_Construction")]
        [HttpDelete("{name}")]
        public async Task<ActionResult<StandardResponse<ConstructionRequest>>> DeleteConstructionLogic(string name)
        {
            var existingConstruction = await _constructionService.FindByName(name);
            var deletedConstruction = await _constructionService.DeleteLogic(existingConstruction!.Name);
            var response = ConstructionMapper.ToResponse(deletedConstruction);
            return Ok(new StandardResponse<ConstructionResponse>(true, "Construcción eliminada exitosamente", response));

        }

    }
}