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
    [Route("api/v1/providers")]
    public class ProviderController : Controller
    {

        private readonly IProviderService _providerService;
        private readonly IValidator<ProviderRequest> _providerValidation;

        public ProviderController(
            IProviderService providerService,
            IValidator<ProviderRequest> providerValidation
        )
        {
            _providerService = providerService;
            _providerValidation = providerValidation;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<StandardResponse<PaginatedResponse<ProviderResponse>>>> FindAllProviders([FromQuery] ProviderFilter filters)
        {

            var providers = await _providerService.FindAll(filters);
            var providerResponse = providers.Items.Select(c => ProviderMapper.ToResponse(c)).ToList();

            var paginatedResponse = new PaginatedResponse<ProviderResponse>
            {
                Items = providerResponse,
                PageIndex = providers.PageIndex,
                PageSize = providers.PageSize,
                TotalItems = providers.TotalItems,
                TotalPages = providers.TotalPages
            };

            return Ok(new StandardResponse<PaginatedResponse<ProviderResponse>>(
                Success: true,
                Message: "Proveedores obtenidos exitosamente",
                Data: paginatedResponse
            ));

        }

        [AllowAnonymous]
        [HttpGet("{cuit:long}")]
        public async Task<ActionResult<StandardResponse<ProviderResponse>>> FindProviderByCuit(long cuit)
        {
            var provider = await _providerService.FindByCuit(cuit);
            var response = ProviderMapper.ToResponse(provider!);
            return Ok(new StandardResponse<ProviderResponse>(true, "Proveedor obtenido exitosamente", response));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<StandardResponse<ProviderResponse>>> CreatedProvider(
            [FromBody] ProviderRequest request
        )
        {

            var validationResult = await _providerValidation.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var validationErrors = string.Join("; ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                var errors = new ErrorDetails(400, "Validacion fallida", HttpContext.Request.Path, validationErrors);
                return new StandardResponse<ProviderResponse>(false, "Ah ocurrido un error", null, errors, 400);
            }

            var newProvider = await _providerService.Save(request);
            var response = ProviderMapper.ToResponse(newProvider);

            return Created(string.Empty, new StandardResponse<ProviderResponse>(true, "Proveedor creado exitosamente", response, null, 201));

        }

        [AllowAnonymous]
        [HttpDelete("permanent/{cuit:long}")]
        public async Task<ActionResult<StandardResponse<ProviderResponse>>> DeleteProvider(long cuit)
        {
            var existingProvider = await _providerService.FindByCuit(cuit);
            var deletedProvider = await _providerService.DeletePermanent(existingProvider!.Cuit);
            var response = ProviderMapper.ToResponse(deletedProvider);
            return Ok(new StandardResponse<ProviderResponse>(true, "Proveedor eliminado exitosamente", response));
        }

        [AllowAnonymous]
        [HttpDelete("{cuit:long}")]
        public async Task<ActionResult<StandardResponse<ProviderRequest>>> DeleteProviderLogic(long cuit)
        {
            var existingProvider = await _providerService.FindByCuit(cuit);
            var deletedProvider = await _providerService.DeleteLogic(existingProvider!.Cuit);
            var response = ProviderMapper.ToResponse(deletedProvider);
            return Ok(new StandardResponse<ProviderResponse>(true, "Proveedor eliminado exitosamente", response));

        }

        [AllowAnonymous]
        [HttpPut("{cuit:long}")]
        public async Task<ActionResult<StandardResponse<ProviderResponse>>> UpdateProvider(
            [FromBody] ProviderRequest request,
            long cuit   
        )
        {

            var existingProvider = await _providerService.FindByCuit(cuit);

            var validationResult = await _providerValidation.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var validationErrors = string.Join("; ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                var errors = new ErrorDetails(400, "Validacion fallida", HttpContext.Request.Path, validationErrors);
                return new StandardResponse<ProviderResponse>(false, "Ah ocurrido un error", null, errors, 400);
            }

            var providerToUpdate = ProviderMapper.ToEntityForUpdate(request, existingProvider!);

            var updatedProvider = await _providerService.Update(providerToUpdate, cuit);
            var response = ProviderMapper.ToResponse(updatedProvider);

            return Ok(new StandardResponse<ProviderResponse>(true, "Proveedor actualizado exitosamente", response));

        }

        [AllowAnonymous]
        [HttpPatch("{cuit:long}")]
        public async Task<ActionResult<StandardResponse<ProviderResponse>>> PartiallyUpdateProvider(
            long cuit,
            [FromBody] JsonPatchDocument<ProviderRequest> patchDoc
        )
        {

            if (patchDoc == null)
            {
                var errors = new ErrorDetails(400, "Documento de parcheo nulo", HttpContext.Request.Path, "El documento de parcheo no puede ser nulo.");
                return BadRequest(new StandardResponse<ProviderResponse>(false, "Ah ocurrido un error", null, errors));
            }

            var existingProvider = await _providerService.FindByCuit(cuit);

            var providerToPatch = ProviderMapper.ToRequest(existingProvider!);

            var modelState = new ModelStateDictionary();
            patchDoc.ApplyTo(providerToPatch, modelState);
            if (!modelState.IsValid)
            {
                var errors = string.Join("; ", modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                var errorDetails = new ErrorDetails(400, "Error de validacion", HttpContext.Request.Path, errors);
                return BadRequest(new StandardResponse<ProviderResponse>(false, "Ah ocurrido un error", null, errorDetails));
            }

            var validationResult = await _providerValidation.ValidateAsync(providerToPatch);

            if (!validationResult.IsValid)
            {
                var validationErrors = string.Join("; ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                var errors = new ErrorDetails(400, "Validacion fallida", HttpContext.Request.Path, validationErrors);
                return new StandardResponse<ProviderResponse>(false, "Ah ocurrido un error", null, errors, 400);
            }

            var provider = ProviderMapper.ToEntityForPatch(providerToPatch, existingProvider!);

            var updatedProvider = await _providerService.UpdatePartial(provider, cuit);

            var response = ProviderMapper.ToResponse(updatedProvider);

            return Ok(new StandardResponse<ProviderResponse>(true, "Proveedor actualizado parcialmente con exito", response));

        }

    }
}
