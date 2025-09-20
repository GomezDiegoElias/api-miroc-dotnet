using org.apimiroc.core.entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.apimiroc.core.data.Repositories.Imp
{
    public interface IRoleRepository
    {
        public Task<Role?> FindByName(string name);
        public Task<Role> Save(Role role);
        public Task<Role> UpdatePermissions(string roleName, IEnumerable<string> addPermissions, IEnumerable<string> removePermissions);
        public Task<List<Role>> FindAll(bool includePermissions = false);
    }
}
