using Microsoft.EntityFrameworkCore;
using org.apimiroc.core.data.Repositories.Imp;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.entities.Exceptions;
using org.apimiroc.core.shared.Dto.General;

namespace org.apimiroc.core.data.Repositories
{
    public class UserRepository : IUserRepository
    {

        private readonly AppDbContext _context;
        private readonly IPaginationRepository _paginationRepository;

        public UserRepository(AppDbContext context, IPaginationRepository paginationRepository)
        {
            _context = context;
            _paginationRepository = paginationRepository;
        }

        public async Task<User> DeleteLogic(long dni)
        {

            var user = await _context.Users
                .Include(u => u.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Dni == dni);

            if (user == null) throw new UserNotFoundException($"Usuario con DNI {dni} no existe");

            user.Status = Status.DELETED;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return user;

        }

        public async Task<User> DeletePermanent(long dni)
        {

            var user = await _context.Users
                .Include(u => u.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Dni == dni);

            if (user == null) throw new UserNotFoundException($"Usuario con DNI {dni} no existe");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user;

        }

        public async Task<PaginatedResponse<User>> FindAll(int pageIndex, int pageSize)
        {
            return await _paginationRepository.ExecutePaginationAsync(
                "getUserPagination",
                reader => new User
                {
                    Id = reader["id"].ToString() ?? string.Empty,
                    Dni = Convert.ToInt64(reader["dni"]),
                    Email = reader["email"].ToString() ?? string.Empty,
                    FirstName = reader["first_name"].ToString() ?? string.Empty,
                    LastName = reader["last_name"].ToString() ?? string.Empty,
                    Role = new Role(reader["role"].ToString() ?? string.Empty, Enumerable.Empty<string>()),
                    Status = Enum.Parse<Status>(reader["status"].ToString() ?? string.Empty)
                },
                pageIndex,
                pageSize
            );
        }

        public async Task<User?> FindByDni(long dni)
        {
            return await _context.Users
                .Include(u => u.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Dni == dni);
        }

        public async Task<User?> FindByEmail(string email)
        {
            return await _context.Users
                .Include(u => u.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> Save(User user)
        {

            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Name == user.Role.Name);

            if (role == null) throw new RoleNotFoundException(user.Role.Name);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;

        }

        public async Task<User> Update(User user, long dniOld)
        {

            var existingUser = await _context.Users
                .Include(u => u.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Dni == dniOld);

            if (existingUser == null) throw new UserNotFoundException($"Usuario con DNI {dniOld} no existe");

            // Actualiza los campos del usuario existente
            existingUser.Dni = user.Dni;
            existingUser.Email = user.Email;
            existingUser.Hash = user.Hash;
            existingUser.Salt = user.Salt;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Status = user.Status;

            // Buscar el RoleEntity correspondiente
            var roleEntity = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == user.Role.Name);

            if (roleEntity == null) throw new RoleNotFoundException(user.Role.Name);

            existingUser.RoleId = roleEntity.Id;
            existingUser.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return existingUser;

        }

        public async Task<User> UpdatePartial(User user, long dniOld)
        {

            var existingUser = await _context.Users
                .Include(u => u.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Dni == dniOld);

            if (existingUser == null) throw new UserNotFoundException($"Usuario con DNI {dniOld} no existe");

            // Actualiza solo los campos que no son nulos en el objeto user
            existingUser.Dni = user.Dni;
            existingUser.Email = user.Email;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Status = user.Status;

            if (!string.IsNullOrEmpty(user.Hash) && !string.IsNullOrEmpty(user.Salt))
            {
                existingUser.Hash = user.Hash;
                existingUser.Salt = user.Salt;
            }

            // Buscar el RoleEntity correspondiente
            var roleEntity = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == user.Role.Name);

            if (roleEntity == null) throw new RoleNotFoundException(user.Role.Name);

            existingUser.RoleId = roleEntity.Id;

            await _context.SaveChangesAsync();

            return existingUser;

        }
    }
}
