using Azure.Core;
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
    [Route("api/v1/constructions")]
    public class ConstructionController : Controller
    {

        private readonly IConstructionService _constructionService;
        private readonly IValidator<ConstructionRequest> _constructionValidation;

        public ConstructionController(
            IConstructionService constructionService,
            IValidator<ConstructionRequest> constructionValidation
        )
        {
            _constructionService = constructionService;
            _constructionValidation = constructionValidation;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<StandardResponse<PaginatedResponse<ConstructionResponse>>>> FindAllConstructions([FromQuery] ConstructionFilter filters)
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

        [AllowAnonymous]
        [HttpGet("{name}")]
        public async Task<ActionResult<StandardResponse<ConstructionResponse>>> FindConstructionByName(string name)
        {
            var construction = await _constructionService.FindByName(name);
            var response = ConstructionMapper.ToResponse(construction!);
            return Ok(new StandardResponse<ConstructionResponse>(true, "Construcción obtenida exitosamente", response));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<StandardResponse<ConstructionResponse>>> CreatedProvider(
            [FromBody] ConstructionRequest request
        )
        {

            var validationResult = await _constructionValidation.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var validationErrors = string.Join("; ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                var errors = new ErrorDetails(400, "Validacion fallida", HttpContext.Request.Path, validationErrors);
                return new StandardResponse<ConstructionResponse>(false, "Ah ocurrido un error", null, errors, 400);
            }

            var newConstruction = await _constructionService.Save(request);
            var response = ConstructionMapper.ToResponse(newConstruction);

            return Created(string.Empty, new StandardResponse<ConstructionResponse>(true, "construcción creada exitosamente", response, null, 201));

        }

        [AllowAnonymous]
        [HttpDelete("permanent/{name}")]
        public async Task<ActionResult<StandardResponse<ConstructionResponse>>> DeleteConstruction(string name)
        {
            var existingConstruction = await _constructionService.FindByName(name);
            var deletedConstruction = await _constructionService.DeletePermanent(existingConstruction!.Name);
            var response = ConstructionMapper.ToResponse(deletedConstruction);
            return Ok(new StandardResponse<ConstructionResponse>(true, "Construcción eliminada exitosamente", response));
        }

        [AllowAnonymous]
        [HttpDelete("{name}")]
        public async Task<ActionResult<StandardResponse<ConstructionRequest>>> DeleteConstructionLogic(string name)
        {
            var existingConstruction = await _constructionService.FindByName(name);
            var deletedConstruction = await _constructionService.DeleteLogic(existingConstruction!.Name);
            var response = ConstructionMapper.ToResponse(deletedConstruction);
            return Ok(new StandardResponse<ConstructionResponse>(true, "Construcción eliminada exitosamente", response));

        }

        [AllowAnonymous]
        [HttpPut("{name}")]
        public async Task<ActionResult<StandardResponse<ConstructionResponse>>> UpdateConstruction(
            [FromBody] ConstructionRequest request,
            string name
        )
        {

            var existingConstruction = await _constructionService.FindByName(name);

            var validationResult = await _constructionValidation.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var validationErrors = string.Join("; ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                var errors = new ErrorDetails(400, "Validacion fallida", HttpContext.Request.Path, validationErrors);
                return new StandardResponse<ConstructionResponse>(false, "Ah ocurrido un error", null, errors, 400);
            }

            var constructionToUpdate = ConstructionMapper.ToEntityForUpdate(request, existingConstruction!);

            var updatedConstruction = await _constructionService.Update(constructionToUpdate, name);
            var response = ConstructionMapper.ToResponse(updatedConstruction);

            return Ok(new StandardResponse<ConstructionResponse>(true, "Construcción actualizada exitosamente", response));

        }

        [AllowAnonymous]
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

            var validationResult = await _constructionValidation.ValidateAsync(constructionToPatch);

            if (!validationResult.IsValid)
            {
                var validationErrors = string.Join("; ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                var errors = new ErrorDetails(400, "Validacion fallida", HttpContext.Request.Path, validationErrors);
                return new StandardResponse<ConstructionResponse>(false, "Ah ocurrido un error", null, errors, 400);
            }

            var construction = ConstructionMapper.ToEntityForPatch(constructionToPatch, existingConstruction!);

            var updatedConstruction = await _constructionService.UpdatePartial(construction, name);

            var response = ConstructionMapper.ToResponse(updatedConstruction);

            return Ok(new StandardResponse<ConstructionResponse>(true, "Construcción actualizada parcialmente con éxito", response));

        }

    }
}