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

namespace org.apimiroc.app.Controllers
{
    [ApiController]
    public class ConstructionController : Controller
    {

        private readonly IConstructionService _constructionService;
        private readonly IClientService _clientService;

        public ConstructionController(IConstructionService constructionService, IClientService clientService)
        {
            _constructionService = constructionService;
            _clientService = clientService;
        }

        // ░░░░░░░░░░░░░░░░░░░░░░░░░░░ ENDPOINTS VERSION 1 - RELACIONES CON CLAVES UNICAS ░░░░░░░░░░░░░░░░░░░░░░░░░░░

        [AllowAnonymous]
        [HttpGet("api/v1/constructions")]
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

        [HttpPost("api/v1/constructions")]
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

        [HttpGet("api/v1/constructions/by-name/{name}")]
        public async Task<ActionResult<StandardResponse<ConstructionResponse>>> FindByName(string name)
        {
            var construction = await _constructionService.FindByName(name);
            var response = ConstructionMapper.ToResponse(construction!);
            return Ok(new StandardResponse<ConstructionResponse>(true, "Construcción obtenida exitosamente", response));
        }

        [AllowAnonymous]
        [HttpGet("api/v1/constructions/{id}")]
        public async Task<ActionResult<StandardResponse<ConstructionResponse>>> FindConstructionById(string id)
        {
            var construction = await _constructionService.FindById(id);
            var response = ConstructionMapper.ToResponse(construction!);
            return Ok(new StandardResponse<ConstructionResponse>(true, "Construcción obtenida exitosamente", response));
        }

        [AllowAnonymous]
        [HttpPut("api/v1/constructions/{name}")]
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

        [AllowAnonymous]
        [HttpPatch("api/v1/constructions/{name}")]
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

        // ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░ ENDPOINTS VERSION 2 - RELACIONES CON ID ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░

        [AllowAnonymous]
        [HttpGet("api/v2/constructions")]
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

        [AllowAnonymous]
        [HttpGet("api/v2/constructions/by-name/{name}")]
        public async Task<ActionResult<StandardResponse<ConstructionResponseV2>>> FindConstructionByNameV2(string name)
        {
            var construction = await _constructionService.FindByName(name);
            var response = ConstructionMapper.ToResponseV2(construction!);
            return Ok(new StandardResponse<ConstructionResponseV2>(true, "Construcción obtenida exitosamente", response));
        }

        [AllowAnonymous]
        [HttpGet("api/v2/constructions/{id}")]
        public async Task<ActionResult<StandardResponse<ConstructionResponseV2>>> FindConstructionByIdV2(string id)
        {
            var construction = await _constructionService.FindById(id);
            var response = ConstructionMapper.ToResponseV2(construction!);
            return Ok(new StandardResponse<ConstructionResponseV2>(true, "Construcción obtenida exitosamente", response));
        }

        [AllowAnonymous]
        [HttpPost("api/v2/constructions")]
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

        [AllowAnonymous]
        [HttpDelete("api/v2/constructions/permanent/{name}")]
        public async Task<ActionResult<StandardResponse<ConstructionResponseV2>>> DeleteConstruction(string name)
        {
            var existingConstruction = await _constructionService.FindByName(name);
            var deletedConstruction = await _constructionService.DeletePermanent(existingConstruction!.Name);
            var response = ConstructionMapper.ToResponseV2(deletedConstruction);
            return Ok(new StandardResponse<ConstructionResponseV2>(true, "Construcción eliminada exitosamente", response));
        }

        [AllowAnonymous]
        [HttpDelete("api/v2/constructions/{name}")]
        public async Task<ActionResult<StandardResponse<ConstructionRequestV2>>> DeleteConstructionLogic(string name)
        {
            var existingConstruction = await _constructionService.FindByName(name);
            var deletedConstruction = await _constructionService.DeleteLogic(existingConstruction!.Name);
            var response = ConstructionMapper.ToResponseV2(deletedConstruction);
            return Ok(new StandardResponse<ConstructionResponseV2>(true, "Construcción eliminada exitosamente", response));

        }

        [AllowAnonymous]
        [HttpPut("api/v2/constructions/{name}")]
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

        [AllowAnonymous]
        [HttpPatch("api/v2/constructions/{name}")]
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

    }
}