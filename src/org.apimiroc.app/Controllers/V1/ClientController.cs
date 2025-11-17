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

namespace org.apimiroc.app.Controllers.V1
{
    [ApiController]
    [Route("api/[controller]")]
    [Route("api/[controller]s")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Route("api/v{version:ApiVersion}/[controller]s")]
    [ApiVersion("1.0")]
    public class ClientController : ControllerBase
    {

        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [Authorize(Policy = "CanREAD_Client")]
        [HttpGet]
        public async Task<ActionResult<StandardResponse<PaginatedResponse<ClientResponse>>>> FindAllClients([FromQuery] ClientFilter filters)
        {

            var clients = await _clientService.FindAll(filters);
            var clientResponse = clients.Items.Select(c => ClientMapper.ToResponse(c)).ToList();

            var paginatedResponse = new PaginatedResponse<ClientResponse>
            {
                Items = clientResponse,
                PageIndex = clients.PageIndex,
                PageSize = clients.PageSize,
                TotalItems = clients.TotalItems,
                TotalPages = clients.TotalPages
            };

            return Ok(new StandardResponse<PaginatedResponse<ClientResponse>>(
                Success: true,
                Message: "Clientes obtenidos exitosamente",
                Data: paginatedResponse
            ));

        }

        [Authorize(Policy = "CanREAD_Client")]
        [HttpGet("{dni:long}")]
        public async Task<ActionResult<StandardResponse<ClientResponse>>> FindClientByDni(long dni)
        {
            var client = await _clientService.FindByDni(dni);
            var response = ClientMapper.ToResponse(client!);
            return Ok(new StandardResponse<ClientResponse>(true, "Cliente obtenido exitosamente", response));
        }

        [Authorize(Policy = "CanREAD_Client")]
        [HttpGet("{id}")]
        public async Task<ActionResult<StandardResponse<ClientResponse>>> FindClientById(string id)
        {
            var client = await _clientService.FindById(id);
            var response = ClientMapper.ToResponse(client!);
            return Ok(new StandardResponse<ClientResponse>(true, "Cliente obtenido exitosamente", response));
        }

        [Authorize(Policy = "CanCREATE_Client")]
        [HttpPost]
        public async Task<ActionResult<StandardResponse<ClientResponse>>> CreatedClient(
            [FromBody] ClientRequest request
        )
        {

            var clientToCreate = ClientMapper.ToEntity(request);
            var newUser = await _clientService.Save(clientToCreate);
            var response = ClientMapper.ToResponse(newUser);

            return Created(string.Empty, new StandardResponse<ClientResponse>(true, "Cliente creado exitosamente", response, null, 201));

        }

        [Authorize(Policy = "CanDELETE_Client")]
        [HttpDelete("permanent/{dni:long}")]
        public async Task<ActionResult<StandardResponse<ClientResponse>>> DeleteClient(long dni)
        {
            var existingClient = await _clientService.FindByDni(dni);
            var deletedClient = await _clientService.DeletePermanent(existingClient!.Dni);
            var response = ClientMapper.ToResponse(deletedClient);
            return Ok(new StandardResponse<ClientResponse>(true, "Cliente eliminado exitosamente", response));
        }

        [Authorize(Policy = "CanDELETE_Client")]
        [HttpDelete("{dni:long}")]
        public async Task<ActionResult<StandardResponse<ClientRequest>>> DeleteClientLogic(long dni)
        {
            var existingClient = await _clientService.FindByDni(dni);
            var deletedClient = await _clientService.DeleteLogic(existingClient!.Dni);
            var response = ClientMapper.ToResponse(deletedClient);
            return Ok(new StandardResponse<ClientResponse>(true, "Cliente eliminado exitosamente", response));

        }

        [Authorize(Policy = "CanUPDATE_Client")]
        [HttpPut("{dni:long}")]
        public async Task<ActionResult<StandardResponse<ClientResponse>>> UpdateClient(
            [FromBody] ClientRequest request,
            long dni
        )
        {

            var existingClient = await _clientService.FindByDni(dni);

            var clientToUpdate = ClientMapper.ToEntityForUpdate(request, existingClient!);

            var updatedClient = await _clientService.Update(clientToUpdate, dni);
            var response = ClientMapper.ToResponse(updatedClient);

            return Ok(new StandardResponse<ClientResponse>(true, "Cliente actualizado exitosamente", response));

        }

        [Authorize(Policy = "CanUPDATE_Client")]
        [HttpPatch("{dni:long}")]
        public async Task<ActionResult<StandardResponse<ClientResponse>>> PartiallyUpdateClient(
            long dni,
            [FromBody] JsonPatchDocument<ClientRequest> patchDoc
        )
        {

            if (patchDoc == null)
            {
                var errors = new ErrorDetails(400, "Documento de parcheo nulo", HttpContext.Request.Path, "El documento de parcheo no puede ser nulo.");
                return BadRequest(new StandardResponse<UserResponse>(false, "Ah ocurrido un error", null, errors));
            }

            var existingClient = await _clientService.FindByDni(dni);

            var clientToPatch = ClientMapper.ToRequest(existingClient!);

            var modelState = new ModelStateDictionary();
            patchDoc.ApplyTo(clientToPatch, modelState);
            if (!modelState.IsValid)
            {
                var errors = string.Join("; ", modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                var errorDetails = new ErrorDetails(400, "Error de validacion", HttpContext.Request.Path, errors);
                return BadRequest(new StandardResponse<UserResponse>(false, "Ah ocurrido un error", null, errorDetails));
            }

            var client = ClientMapper.ToEntityForPatch(clientToPatch, existingClient!);

            var updatedClient = await _clientService.UpdatePartial(client, dni);

            var response = ClientMapper.ToResponse(updatedClient);

            return Ok(new StandardResponse<ClientResponse>(true, "Cliente actualizado parcialmente con exito", response));

        }

    }
}
