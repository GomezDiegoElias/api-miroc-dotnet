using Microsoft.IdentityModel.Tokens;
using org.apimiroc.core.business.Services.Imp;
using org.apimiroc.core.data.Repositories.Imp;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.entities.Enums;
using org.apimiroc.core.entities.Exceptions;
using org.apimiroc.core.shared;
using org.apimiroc.core.shared.Dto.Request;
using org.apimiroc.core.shared.Dto.Response;
using org.apimiroc.core.shared.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

        public async Task<AuthResponse> Login(LoginRequest request)
        {

            var user = await _userRepository.FindByEmail(request.Email);

            if (user == null) throw new UnauthorizedAccessException("Credenciales invalidas");

            bool passwordValid = PasswordUtils.VerifyPassword(request.Password, user.Hash, user.Salt);

            if (!passwordValid) throw new UnauthorizedAccessException("Credenciales invalidas");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var claims = new List<Claim>
            {
                new Claim("id", user.Id),
                new Claim("dni", user.Dni.ToString()),
                new Claim("email", user.Email),
                new Claim("role", user.Role.Name)
                // + Claims
            };

            List<string> permissions = user.Role.RolePermissions
                .Select(rp => rp.Permission.Name)
                .ToList();

            foreach (var perm in permissions)
            {
                claims.Add(new Claim("permission", perm));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpirationMinutes),
                Issuer = _jwtConfig.Issuer,
                Audience = _jwtConfig.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token); 

            return new AuthResponse(
                AccessToken: tokenString,
                RefreshToken: "Proximamente"
            );

        }

        public async Task<AuthResponse> Register(RegisterRequest request)
        {

            if (await _userRepository.FindByEmail(request.Email) != null)
                throw new UserNotFoundException($"Correo electronico '{request.Email}' ya existe");

            if (await _userRepository.FindByDni(request.Dni) != null)
                throw new UserNotFoundException($"Numero de DNI '{request.Dni}' ya existe");

            string salt = PasswordUtils.GenerateRandomSalt();
            string hashedPassword = PasswordUtils.HashPasswordWithSalt(request.Password, salt);

            var roleEntity = await _roleRepository.FindByName("PRESUPUESTISTA");
            if (roleEntity == null) throw new RoleNotFoundException("PRESUPUESTISTA");

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
