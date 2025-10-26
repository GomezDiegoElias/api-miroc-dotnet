using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using org.apimiroc.app.Mappers;
using org.apimiroc.core.business.Services.Imp;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;
using org.apimiroc.core.shared.Dto.Request;
using org.apimiroc.core.shared.Dto.Response;

namespace org.apimiroc.app.Controllers
{
    [ApiController]
    [Route("api/v1/concepts")]
    public class ConceptController : Controller
    {

        private readonly IConceptService _service;
        private readonly IValidator<ConceptRequest> _conceptValidation;

        public ConceptController(IConceptService service, IValidator<ConceptRequest> conceptValidation)
        {
            _service = service;
            _conceptValidation = conceptValidation;
        }

        [HttpGet]
        public async Task<ActionResult<StandardResponse<List<ConceptResponse>>>> GetAllConcepts([FromQuery] ConceptFilter filters)
        {
            var concepts = await _service.FindAll(filters);
            var response = concepts.Items.Select(c => ConceptMapper.ToResponse(c)).ToList();
            return Ok(new StandardResponse<List<ConceptResponse>>(
                true,
                "Conceptos de movimientos obtenidos exitosamente.",
                response,
                null,
                200
            ));
        }

        [HttpPost]
        public async Task<ActionResult<StandardResponse<ConceptResponse>>> CreateConcept([FromBody] ConceptRequest request)
        {
            var validationResult = await _conceptValidation.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var validationErrors = string.Join("; ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                var errors = new ErrorDetails(400, "Validación fallida", HttpContext.Request.Path, validationErrors);
                return BadRequest(new StandardResponse<ConceptResponse>(false, "Ah acurrido un error", null, errors, 400));
            }

            var conceptCaptured = ConceptMapper.ToEntity(request);
            var savedConcept = await _service.Save(conceptCaptured);
            var response = ConceptMapper.ToResponse(savedConcept);

            return Created(string.Empty, new StandardResponse<ConceptResponse>(
                true,
                "Concepto de movimiento creado exitosamente",
                response,
                null,
                201
            ));

        }

    }
}
