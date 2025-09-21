using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using org.apimiroc.app.Mappers;
using org.apimiroc.core.business.Services.Imp;
using org.apimiroc.core.entities.Exceptions;
using org.apimiroc.core.shared.Dto.General;
using org.apimiroc.core.shared.Dto.Request;
using org.apimiroc.core.shared.Dto.Response;

namespace org.apimiroc.app.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UserController : Controller
    {

        private readonly IUserService _userService;
        private readonly IValidator<UserUpdateRequest> _userUpdateValidator;
        private readonly IValidator<UserCreateRequest> _userCreateValidator;

        public UserController(
            IUserService userService,
            IValidator<UserUpdateRequest> userUpdateValidator,
            IValidator<UserCreateRequest> userCreateValidator
        )
        {
            _userService = userService;
            _userUpdateValidator = userUpdateValidator;
            _userCreateValidator = userCreateValidator;
        }

        //[Authorize] // Requiere autenticacion
        //[Authorize(Roles = "ADMIN")] // Requiere autenticacion y que tenga el rol de admin
        //[Authorize(Policy = "CanCREATE_Client")] // Requiere autenticacion y el permiso de crear cliente
        [HttpGet]
        public async Task<ActionResult<StandardResponse<PaginatedResponse<UserResponse>>>> GetAllUsers(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10
        )
        {

            var users = await _userService.FindAllUsers(page, pageSize);

            var userResponse = users.Items.Select(u => UserMapper.ToResponse(u)).ToList();

            var paginatedResponse = new PaginatedResponse<UserResponse>
            {
                Items = userResponse,
                PageIndex = users.PageIndex,
                PageSize = users.PageSize,
                TotalItems = users.TotalItems,
                TotalPages = users.TotalPages
            };

            var response = new StandardResponse<PaginatedResponse<UserResponse>>(
                Success: true,
                Message: "Usuarios obtenidos exitosamente",
                Data: paginatedResponse
            );

            return Ok(response);

        }

        [AllowAnonymous]
        [HttpGet("{dni:long}")]
        public async Task<ActionResult<StandardResponse<UserResponse>>> GetUserByDni(long dni)
        {
            var user = await _userService.FindByDni(dni);
            var response = UserMapper.ToResponse(user!);
            return Ok(new StandardResponse<UserResponse>(true, "Usuario obtenido exitosamente", response));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<StandardResponse<UserResponse>>> CreateUser([FromBody] UserCreateRequest request)
        {

            var validationResult = await _userCreateValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var validationErrors = string.Join("; ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                var errors = new ErrorDetails(400, "Validacion fallida", HttpContext.Request.Path, validationErrors);
                return new StandardResponse<UserResponse>(false, "Ah ocurrido un error", null, errors);
            }

            var newUser = await _userService.SaveCustomUser(request);

            var response = new StandardResponse<UserResponse>(
                Success: true,
                Message: "Usuario creado exitosamente",
                Data: UserMapper.ToResponse(newUser)
            );

            return Created(string.Empty, response);

        }

        [AllowAnonymous]
        [HttpPut("{dni:long}")]
        public async Task<ActionResult<StandardResponse<UserResponse>>> UpdateUser(
            [FromBody] UserUpdateRequest request,
            long dni
        )
        {

            var existingUser = await _userService.FindByDni(dni);
            if (existingUser == null) throw new UserNotFoundException(dni.ToString());

            // Validar el request
            var validationResult = await _userUpdateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var validationErrors = string.Join("; ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                var errors = new ErrorDetails(400, "Validacion fallida", HttpContext.Request.Path, validationErrors);
                return new StandardResponse<UserResponse>(false, "Ah ocurrido un error", null, errors);
            }

            var userToUpdate = UserMapper.ToEntityForUpdate(request, existingUser);

            var updatedUser = await _userService.Update(userToUpdate);
            var response = UserMapper.ToResponse(updatedUser);

            return Ok(new StandardResponse<UserResponse>(
                Success: true,
                Message: "Usuario actualizado exitosamente",
                Data: response
            ));

        }

        [HttpPatch("{dni:long}")]
        public async Task<ActionResult<StandardResponse<UserResponse>>> PartiallyUpdateUser(
            long dni,
            [FromBody] JsonPatchDocument<UserUpdateRequest> patchDoc)
        {

            if (patchDoc == null)
            {
                var errors = new ErrorDetails(400, "Documento de parcheo nulo", HttpContext.Request.Path, "El documento de parcheo no puede ser nulo.");
                return BadRequest(new StandardResponse<UserResponse>(false, "Ah ocurrido un error", null, errors));
            }

            var existingUser = await _userService.FindByDni(dni)
                ?? throw new UserNotFoundException($"Usuario con DNI {dni} no existe");

            // Mapea entidad a DTO request
            var userToPatch = UserMapper.ToRequest(existingUser);

            // Aplica patch
            var modelState = new ModelStateDictionary();
            patchDoc.ApplyTo(userToPatch, modelState);
            if (!modelState.IsValid)
            {
                var errors = string.Join("; ", modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                var errorDetails = new ErrorDetails(400, "Error de validacion", HttpContext.Request.Path, errors);
                return BadRequest(new StandardResponse<UserResponse>(false, "Ah ocurrido un error", null, errorDetails));
            }

            //var validationResult = await _userUpdateValidator.ValidateAsync(userToPatch);
            //if (!validationResult.IsValid)
            //{
            //    var validationErrors = string.Join("; ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
            //    var errors = new ErrorDetails(400, "Validacion fallida", HttpContext.Request.Path, validationErrors);
            //    return new StandardResponse<UserResponse>(false, "Ah ocurrido un error", null, errors);
            //}

            // Usa el mapper especifico para PATCH
            var userDomain = UserMapper.ToEntityForPatch(userToPatch, existingUser);

            // Guarda los cambios
            var updatedUser = await _userService.UpdatePartial(userDomain);

            // Mapea a response DTO
            var response = UserMapper.ToResponse(updatedUser);

            return Ok(new StandardResponse<UserResponse>(
                Success: true,
                Message: "Usuario actualizado parcialmente con éxito",
                Data: response
            ));

        }

    }
}
