using org.apimiroc.core.business.Services.Imp;
using org.apimiroc.core.data.Repositories.Imp;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.entities.Exceptions;
using org.apimiroc.core.shared;
using org.apimiroc.core.shared.Dto.Request;
using org.apimiroc.core.shared.Dto.Response;
using org.apimiroc.core.shared.Utils;

namespace org.apimiroc.core.business.Services
{
    public class AuthService : IAuthService
    {

        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly JwtConfigDto _jwtConfig;

        public AuthService(IUserRepository userRepository, IRoleRepository roleRepository, JwtConfigDto jwtConfig)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _jwtConfig = jwtConfig;
        }

        public Task<AuthResponse> Login(LoginRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthResponse> Register(RegisterRequest request)
        {

            if (await _userRepository.FindByEmail(request.Email) != null)
                throw new UserNotFoundException($"Error: Correo electronico '{request.Email}' ya existe");

            if (await _userRepository.FindByDni(request.Dni) != null)
                throw new UserNotFoundException($"Error: Numero de DNI '{request.Dni}' ya existe");

            string salt = PasswordUtils.GenerateRandomSalt();
            string hashedPassword = PasswordUtils.HashPasswordWithSalt(request.Password, salt);

            var roleEntity = await _roleRepository.FindByName("PRESUPUESTISTA");
            if (roleEntity == null) throw new RoleNotFoundException("Rol PRESUPUESTISTA no existe");

            var newUser = new User
            {
                Id = User.GenerateId(),
                Dni = request.Dni,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Hash = hashedPassword,
                Salt = salt,
                RoleId = roleEntity.Id,   // <-- asigna el FK directamente
                Role = roleEntity,        // <-- opcional, EF lo manejará
                Status = Status.ACTIVE,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _userRepository.Save(newUser);

            AuthResponse response = new AuthResponse(
                //Message: "User created successfully",
                AccessToken: "",
                RefreshToken: ""
            );

            return response;

        }

    }
}
