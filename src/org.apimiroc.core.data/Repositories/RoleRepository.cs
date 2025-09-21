using Microsoft.EntityFrameworkCore;
using org.apimiroc.core.data.Repositories.Imp;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.entities.Exceptions;

namespace org.apimiroc.core.data.Repositories
{
    public class RoleRepository : IRoleRepository
    {

        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        // Retorna todos los roles, con opcion de incluir permisos
        public async Task<List<Role>> FindAll(bool includePermissions = false)
        {
            // El 'IQueryable' permite construir consultas dinamicamente
            // Si se usara 'DbSet' directamente, se ejecutaria la consulta inmediatamente
            // y no se podria agregar condiciones adicionales como 'Include'
            // Por eso se usa 'IQueryable' para construir la consulta y luego ejecutarla con 'ToListAsync'
            IQueryable<Role> query = _context.Roles;

            // Si se indica, incluye los permisos asociados a cada rol
            if (includePermissions)
            {
                query = query
                    .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission);
            }

            // Ejecuta la consulta y obtiene la lista de entidades
            var entities = await query.ToListAsync();

            return entities;

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

        public async Task<Role> UpdatePermissions(string roleName, IEnumerable<string> addPermissions, IEnumerable<string> removePermissions)
        {

            var role = _context.Roles
                                .Include(r => r.RolePermissions)
                                .ThenInclude(rp => rp.Permission)
                                .FirstOrDefault(r => r.Name == roleName);

            if (role == null) throw new RoleNotFoundException(role.Name);

            // Agrega permisos
            foreach (var permName in addPermissions)
            {

                // Busca el permiso en la base de datos
                var permEntity = await _context.Permissions.FirstOrDefaultAsync(p => p.Name == permName);
                if (permEntity == null) throw new ApplicationException($"Permission '{permName}' no existe.");

                // Verifica si ya existe
                if (!role.RolePermissions.Any(rp => rp.PermissionId == permEntity.Id))
                {
                    role.RolePermissions.Add(new RolePermission
                    {
                        Role = role,
                        Permission = permEntity
                    });
                }

            }

            // Quita permisos
            foreach (var permName in removePermissions)
            {

                var rpToRemove = role.RolePermissions.FirstOrDefault(rp => rp.Permission.Name == permName);

                if (rpToRemove != null)
                {
                    role.RolePermissions.Remove(rpToRemove);
                }

            }

            await _context.SaveChangesAsync();

            return role;

        }

    }
}
