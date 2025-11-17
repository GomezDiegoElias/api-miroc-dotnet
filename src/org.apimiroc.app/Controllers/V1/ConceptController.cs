using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [Route("api/v{version:ApiVersion}/[controller]s")] 
    [ApiVersion("1.0")]
    public class ConceptController : ControllerBase
    {

        private readonly IConceptService _service;

        public ConceptController(IConceptService service)
        {
            _service = service;
        }

        [Authorize(Policy = "CanREAD_Concept")]
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

        [Authorize(Policy = "CanCREATE_Concept")]
        [HttpPost]
        public async Task<ActionResult<StandardResponse<ConceptResponse>>> CreateConcept([FromBody] ConceptRequest request)
        {

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

        [Authorize(Policy = "CanREAD_Concept")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<StandardResponse<ConceptResponse>>> GetById(int id)
        {
            var conceptObtained = await _service.FindById(id);
            return Ok(new StandardResponse<ConceptResponse>(
                true,
                "Concepto de movimiento obtenido exitosamente.",
                ConceptMapper.ToResponse(conceptObtained),
                null,
                200
            ));
        }

        [Authorize(Policy = "CanREAD_Concept")]
        [HttpGet("by-name/{name}")]
        public async Task<ActionResult<StandardResponse<ConceptResponse>>> GetByName(string name)
        {
            var conceptObtained = await _service.FindByName(name);
            return Ok(new StandardResponse<ConceptResponse>(
                true,
                "Concepto de movimiento obtenido exitosamente.",
                ConceptMapper.ToResponse(conceptObtained),
                null,
                200
            ));
        }

        [Authorize(Policy = "CanREAD_Concept")]
        [HttpGet("by-type/{type}")]
        public async Task<ActionResult<StandardResponse<ConceptResponse>>> GetByType(string type)
        {
            var conceptObtained = await _service.FindByType(type);
            return Ok(new StandardResponse<ConceptResponse>(
                true,
                "Concepto de movimiento obtenido exitosamente.",
                ConceptMapper.ToResponse(conceptObtained),
                null,
                200
            ));
        }

        [Authorize(Policy = "CanDELETE_Concept")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<StandardResponse<ConceptResponse>>> DeletedLogic(int id)
        {
            var deletedConcept = await _service.Delete(id);
            return Ok(new StandardResponse<ConceptResponse>(
                true,
                "Concepto de movimiento eliminado exitosamente.",
                ConceptMapper.ToResponse(deletedConcept),
                null,
                200
            ));
        }

        [Authorize(Policy = "CanUPDATE_Concept")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<StandardResponse<ConceptResponse>>> UpdateConcept(int id, [FromBody] ConceptRequest request)
        {
            var updatedConcept = await _service.Update(id, request);
            return Ok(new StandardResponse<ConceptResponse>(
                true,
                "Concepto de movimiento actualizado exitosamente.",
                ConceptMapper.ToResponse(updatedConcept),
                null,
                200
            ));
        }

    }
}
