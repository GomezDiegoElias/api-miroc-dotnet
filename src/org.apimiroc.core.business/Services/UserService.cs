using org.apimiroc.core.business.Services.Imp;
using org.apimiroc.core.data.Repositories;
using org.apimiroc.core.data.Repositories.Imp;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.entities.Exceptions;
using org.apimiroc.core.shared.Dto.Filter;
using org.apimiroc.core.shared.Dto.General;
using org.apimiroc.core.shared.Dto.Request;
using org.apimiroc.core.shared.Utils;

namespace org.apimiroc.core.business.Services
{
    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public Task<User> DeleteLogic(long dni)
        {
            throw new NotImplementedException();
        }

        public Task<User> DeletePermanent(long dni)
        {
            throw new NotImplementedException();
        }

        public async Task<PaginatedResponse<User>> FindAllUsers(UserFilter filters)
        {
            return await _userRepository.FindAll(filters);
        }

        public async Task<User?> FindByDni(long dni)
        {
            return await _userRepository.FindByDni(dni)
                ?? throw new UserNotFoundException($"Usuario con DNI '{dni}' no existe");
        }

        public async Task<User> SaveCustomUser(UserCreateRequest request)
        {

            if (await _userRepository.FindByEmail(request.Email) != null)
                throw new UserNotFoundException($"Correo electronico '{request.Email}' ya existe");

            if (await _userRepository.FindByDni(request.Dni) != null)
                throw new UserNotFoundException($"Numero de DNI '{request.Dni}' ya existe");

            string salt = PasswordUtils.GenerateRandomSalt();
            string hashedPassword = PasswordUtils.HashPasswordWithSalt(request.Password, salt);

            var roleEntity = await _roleRepository.FindByName(request.Role);
            if (roleEntity == null)
                throw new RoleNotFoundException(request.Role);

            var userCustom = new User
            {
                Id = User.GenerateId(),
                Dni = request.Dni,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = roleEntity,
                Status = Status.ACTIVE, // Estado ACTIVO por defecto
                Hash = hashedPassword,
                Salt = salt
                //CreatedAt = DateTime.UtcNow,
                //UpdatedAt = DateTime.UtcNow
            };

            //if (string.IsNullOrEmpty(userCustom.Id)) userCustom.Id = User.GenerateId();

            var savedUser = await _userRepository.Save(userCustom);

            return savedUser;

        }

        public Task<User> Update(User user, long dniOld)
        {
            return _userRepository.Update(user, dniOld);
        }

        public Task<User> UpdatePartial(User user, long dniOld)
        {
            return _userRepository.UpdatePartial(user, dniOld);
        }
    }
}
