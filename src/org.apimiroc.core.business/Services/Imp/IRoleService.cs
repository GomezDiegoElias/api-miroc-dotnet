using org.apimiroc.core.entities.Entities;

namespace org.apimiroc.core.business.Services.Imp
{
    public interface IRoleService
    {
        Task<Role> GetRoleByName(string name);
        Task<Role> CreateRole(Role role);
        Task<Role> UpdateRolePermissions(string roleName, IEnumerable<string> addPermissions, IEnumerable<string> removePermissions);
        Task<List<Role>> GetAllRoles(bool includePermissions = false);
    }
}
