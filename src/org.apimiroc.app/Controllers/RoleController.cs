using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using org.apimiroc.app.Mappers;
using org.apimiroc.core.business.Services.Imp;
using org.apimiroc.core.shared.Dto.General;
using org.apimiroc.core.shared.Dto.Response;

namespace org.apimiroc.app.Controllers
{
    [ApiController]
    [Route("api/v1/roles")]
    public class RoleController : Controller
    {

        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<StandardResponse<object>>> GetRoles([FromQuery] bool includePermissions = false)
        {

            var roles = await _roleService.GetAllRoles(includePermissions);

            // Mapea los roles a respuestas DTO
            var data = roles
                .Select(r => RoleMapper.ToResponse(r, includePermissions))
                .ToList();

            return Ok(new StandardResponse<object>(
                true,
                includePermissions ? "Roles con permisos" : "Roles (solo nombres)",
                data,
                null,
                200
            ));

        }

    }
}
