using Microsoft.EntityFrameworkCore;
using org.apimiroc.core.data.Repositories.Imp;
using org.apimiroc.core.entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.apimiroc.core.data.Repositories
{
    public class RoleRepository : IRoleRepository
    {

        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<Role>> FindAll(bool includePermissions = false)
        {
            throw new NotImplementedException();
        }

        public async Task<Role?> FindByName(string name)
        {
            return await _context.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Name == name);
        }

        public async Task<Role> Save(Role role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public Task<Role> UpdatePermissions(string roleName, IEnumerable<string> addPermissions, IEnumerable<string> removePermissions)
        {
            throw new NotImplementedException();
        }
    }
}
