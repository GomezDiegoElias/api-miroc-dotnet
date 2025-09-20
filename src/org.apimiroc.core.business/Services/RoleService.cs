using org.apimiroc.core.business.Services.Imp;
using org.apimiroc.core.data.Repositories.Imp;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.entities.Exceptions;

namespace org.apimiroc.core.business.Services
{
    public class RoleService : IRoleService
    {

        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<Role> CreateRole(Role role)
        {
            return await _roleRepository.Save(role);
        }

        public Task<List<Role>> GetAllRoles(bool includePermissions = false)
        {
            throw new NotImplementedException();
        }

        public async Task<Role> GetRoleByName(string name)
        {
            var role = await _roleRepository.FindByName(name)
                ?? throw new RoleNotFoundException(name);
            return role;
        }

        public Task<Role> UpdateRolePermissions(string roleName, IEnumerable<string> addPermissions, IEnumerable<string> removePermissions)
        {
            throw new NotImplementedException();
        }
    }
}
