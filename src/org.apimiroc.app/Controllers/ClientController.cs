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
    [Route("api/v1/clients")]
    public class ClientController : Controller
    {

        private readonly IClientService _clientService;
        private readonly IValidator<ClientRequest> _clientValidation;

        public ClientController(
            IClientService clientService,
            IValidator<ClientRequest> clientValidation    
        )
        {
            _clientService = clientService;
            _clientValidation = clientValidation;
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
        [HttpGet("{dni:long}")]
        public async Task<ActionResult<StandardResponse<ClientResponse>>> FindClientByDni(long dni)
        {
            var client = await _clientService.FindByDni(dni);
            var response = ClientMapper.ToResponse(client!);
            return Ok(new StandardResponse<ClientResponse>(true, "Cliente obtenido exitosamente", response));
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<StandardResponse<ClientResponse>>> FindClientById(string id)
        {
            var client = await _clientService.FindById(id);
            var response = ClientMapper.ToResponse(client!);
            return Ok(new StandardResponse<ClientResponse>(true, "Cliente obtenido exitosamente", response));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<StandardResponse<ClientResponse>>> CreatedClient(
            [FromBody] ClientRequest request
        )
        {

            var validationResult = await _clientValidation.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var validationErrors = string.Join("; ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                var errors = new ErrorDetails(400, "Validacion fallida", HttpContext.Request.Path, validationErrors);
                return new StandardResponse<ClientResponse>(false, "Ah ocurrido un error", null, errors, 400);
            }

            var newUser = await _clientService.Save(request);
            var response = ClientMapper.ToResponse(newUser);

            return Created(string.Empty, new StandardResponse<ClientResponse>(true, "Cliente creado exitosamente", response, null, 201));

        }

        [AllowAnonymous]
        [HttpDelete("permanent/{dni:long}")]
        public async Task<ActionResult<StandardResponse<ClientResponse>>> DeleteClient(long dni)
        {
            var existingClient = await _clientService.FindByDni(dni);
            var deletedClient = await _clientService.DeletePermanent(existingClient!.Dni);
            var response = ClientMapper.ToResponse(deletedClient);
            return Ok(new StandardResponse<ClientResponse>(true, "Cliente eliminado exitosamente", response));
        }

        [AllowAnonymous]
        [HttpDelete("{dni:long}")]
        public async Task<ActionResult<StandardResponse<ClientRequest>>> DeleteClientLogic(long dni)
        {
            var existingClient = await _clientService.FindByDni(dni);
            var deletedClient = await _clientService.DeleteLogic(existingClient!.Dni);
            var response = ClientMapper.ToResponse(deletedClient);
            return Ok(new StandardResponse<ClientResponse>(true, "Cliente eliminado exitosamente", response));

        }

        [AllowAnonymous]
        [HttpPut("{dni:long}")]
        public async Task<ActionResult<StandardResponse<ClientResponse>>> UpdateClient(
            [FromBody] ClientRequest request,
            long dni
        )
        {

            var existingClient = await _clientService.FindByDni(dni);

            var validationResult = await _clientValidation.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var validationErrors = string.Join("; ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                var errors = new ErrorDetails(400, "Validacion fallida", HttpContext.Request.Path, validationErrors);
                return new StandardResponse<ClientResponse>(false, "Ah ocurrido un error", null, errors, 400);
            }

            var clientToUpdate = ClientMapper.ToEntityForUpdate(request, existingClient!);

            var updatedClient = await _clientService.Update(clientToUpdate, dni);
            var response = ClientMapper.ToResponse(updatedClient);

            return Ok(new StandardResponse<ClientResponse>(true, "Cliente actualizado exitosamente", response));

        }

        [AllowAnonymous]
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

            var validationResult = await _clientValidation.ValidateAsync(clientToPatch);
            if (!validationResult.IsValid)
            {
                var validationErrors = string.Join("; ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                var errors = new ErrorDetails(400, "Validacion fallida", HttpContext.Request.Path, validationErrors);
                return new StandardResponse<ClientResponse>(false, "Ah ocurrido un error", null, errors, 400);
            }

            var client = ClientMapper.ToEntityForPatch(clientToPatch, existingClient!);

            var updatedClient = await _clientService.UpdatePartial(client, dni);

            var response = ClientMapper.ToResponse(updatedClient);

            return Ok(new StandardResponse<ClientResponse>(true, "Cliente actualizado parcialmente con exito", response));

        }

        //[HttpGet("movements")]
        //public async Task<ActionResult<StandardResponse<List<ClientMovementResponse>>>> FindAllMovemets()
        //{

        //    var movements = await _clientService.FindAllMovementsClients();

        //    var response = movements.Select(m => ClientMapper.ToMovementResponse(m)).ToList();

        //    return Ok(new StandardResponse<List<ClientMovementResponse>>(
        //        true,
        //        "Movimientos de clientes obtenidos exitosamente",
        //        response
        //    ));

        //}

        //[HttpGet("{dni:long}/movements")]
        //public async Task<ActionResult<StandardResponse<List<ClientMovementResponse>>>> FindAllMovemets(long dni)
        //{

        //    var movements = await _clientService.FindAllMovementsClientByDni(dni);

        //    var response = movements.Select(m => ClientMapper.ToMovementResponse(m)).ToList();

        //    return Ok(new StandardResponse<List<ClientMovementResponse>>(
        //        true,
        //        "Movimientos de clientes obtenidos exitosamente",
        //        response
        //    ));

        //}

    }
}
