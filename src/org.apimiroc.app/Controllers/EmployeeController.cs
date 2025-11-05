using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using org.apimiroc.app.Mappers;
using org.apimiroc.app.Validations;
using org.apimiroc.core.business.Services;
using org.apimiroc.core.business.Services.Imp;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;
using org.apimiroc.core.shared.Dto.Request;
using org.apimiroc.core.shared.Dto.Response;

namespace org.apimiroc.app.Controllers
{
    [ApiController]
    [Route("api/v1/employees")]
    public class EmployeeController : Controller
    {

        private readonly IEmployeeService _employeeService;
        private readonly IValidator<EmployeeRequest> _employeeValidation;

        public EmployeeController(
            IEmployeeService employeeService,
            IValidator<EmployeeRequest> employeeValidation    
        )
        {
            _employeeService = employeeService;
            _employeeValidation = employeeValidation;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<StandardResponse<EmployeeResponse>>> FindAllEmployees([FromQuery] EmployeeFilter filters)
        {

            var employees = await _employeeService.FindAll(filters);
            var employeeResponse = employees.Items.Select(e => EmployeeMapper.ToResponse(e)).ToList();

            var paginatedResponse = new PaginatedResponse<EmployeeResponse>
            {
                Items = employeeResponse,
                PageIndex = employees.PageIndex,
                PageSize = employees.PageSize,
                TotalItems = employees.TotalItems,
                TotalPages = employees.TotalPages
            };

            return Ok(new StandardResponse<PaginatedResponse<EmployeeResponse>>(true, "Empleados obtenidos exitosamente", paginatedResponse));

        }

        [AllowAnonymous]
        [HttpGet("{dni:long}")]
        public async Task<ActionResult<StandardResponse<EmployeeResponse>>> FindEmployeeByDni(long dni)
        {
            var employee = await _employeeService.FindByDni(dni);
            var response = EmployeeMapper.ToResponse(employee);
            return Ok(new StandardResponse<EmployeeResponse>(true, "Empleado obtenido exitosamente", response));
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<StandardResponse<EmployeeResponse>>> FindEmployeeById(string id)
        {
            var employee = await _employeeService.FindById(id);
            var response = EmployeeMapper.ToResponse(employee);
            return Ok(new StandardResponse<EmployeeResponse>(true, "Empleado obtenido exitosamente", response));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<StandardResponse<EmployeeResponse>>> SaveEmployee([FromBody] EmployeeRequest request)
        {

            var validationResult = await _employeeValidation.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var validationErrors = string.Join("; ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                var errors = new ErrorDetails(400, "Validacion fallida", HttpContext.Request.Path, validationErrors);
                return new StandardResponse<EmployeeResponse>(false, "Ah ocurrido un error", null, errors, 400);
            }

            var employeeToCreate = EmployeeMapper.ToEntity(request);
            var newEmployee = await _employeeService.Save(employeeToCreate);
            var response = EmployeeMapper.ToResponse(newEmployee);

            return Created(string.Empty, new StandardResponse<EmployeeResponse>(true, "Empleado creado correctamente", response, null, 201));

        }

        [AllowAnonymous]
        [HttpDelete("permanent/{dni:long}")]
        public async Task<ActionResult<StandardResponse<EmployeeResponse>>> DeleteEmployee(long dni)
        {
            var existingEmployee = await _employeeService.FindByDni(dni);
            var deletedEmployee = await _employeeService.DeletePermanent(existingEmployee!.Dni);
            var response = EmployeeMapper.ToResponse(deletedEmployee);
            return Ok(new StandardResponse<EmployeeResponse>(true, "Empleado eliminado exitosamente", response));
        }

        [AllowAnonymous]
        [HttpDelete("{dni:long}")]
        public async Task<ActionResult<StandardResponse<EmployeeRequest>>> DeleteClientLogic(long dni)
        {
            var existingEmployee = await _employeeService.FindByDni(dni);
            var deletedEmployee = await _employeeService.DeleteLogic(existingEmployee!.Dni);
            var response = EmployeeMapper.ToResponse(deletedEmployee);
            return Ok(new StandardResponse<EmployeeResponse>(true, "Empleado eliminado exitosamente", response));
        }

        [AllowAnonymous]
        [HttpPut("{dni:long}")]
        public async Task<ActionResult<StandardResponse<EmployeeResponse>>> UpdateEmployee(
            [FromBody] EmployeeRequest request,
            long dni
        )
        {

            var existingClient = await _employeeService.FindByDni(dni);

            var validationResult = await _employeeValidation.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                var validationErrors = string.Join("; ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                var errors = new ErrorDetails(400, "Validacion fallida", HttpContext.Request.Path, validationErrors);
                return new StandardResponse<EmployeeResponse>(false, "Ah ocurrido un error", null, errors, 400);
            }

            var employeeToUpdate = EmployeeMapper.ToEntityForUpdate(request, existingClient!);

            var updatedEmployee = await _employeeService.Update(employeeToUpdate, dni);
            var response = EmployeeMapper.ToResponse(updatedEmployee);

            return Ok(new StandardResponse<EmployeeResponse>(true, "Empleado actualizado exitosamente", response));

        }

        [AllowAnonymous]
        [HttpPatch("{dni:long}")]
        public async Task<ActionResult<StandardResponse<EmployeeResponse>>> PartiallyUpdateEmployee(
            long dni,
            [FromBody] JsonPatchDocument<EmployeeRequest> patchDoc
        )
        {

            if (patchDoc == null)
            {
                var errors = new ErrorDetails(400, "Documento de parcheo nulo", HttpContext.Request.Path, "El documento de parcheo no puede ser nulo.");
                return BadRequest(new StandardResponse<EmployeeResponse>(false, "Ah ocurrido un error", null, errors));
            }

            var existingEmployee = await _employeeService.FindByDni(dni);

            var employeeToPatch = EmployeeMapper.ToRequest(existingEmployee!);

            var modelState = new ModelStateDictionary();
            patchDoc.ApplyTo(employeeToPatch, modelState);
            if (!modelState.IsValid)
            {
                var errors = string.Join("; ", modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                var errorDetails = new ErrorDetails(400, "Error de validacion", HttpContext.Request.Path, errors);
                return BadRequest(new StandardResponse<EmployeeResponse>(false, "Ah ocurrido un error", null, errorDetails));
            }

            var employee = EmployeeMapper.ToEntityForPatch(employeeToPatch, existingEmployee!);

            var updatedEmployee = await _employeeService.UpdatePartial(employee, dni);

            var response = EmployeeMapper.ToResponse(updatedEmployee);

            return Ok(new StandardResponse<EmployeeResponse>(true, "Empleado actualizado parcialmente con exito", response));

        }

    }
}
