using FakeItEasy;
using FluentAssertions;
using org.apimiroc.core.business.Services;
using org.apimiroc.core.data.Repositories.Imp;
using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.entities.Enums;
using org.apimiroc.core.entities.Exceptions;
using org.apimiroc.core.shared;
using org.apimiroc.core.shared.Dto.Request;
using org.apimiroc.core.shared.Utils;

namespace org.apimiroc.app.test.UnitTest
{
    public class AuthServiceTest
    {

        // Convención utilizada para los nombres de los tests: Given_When_Then (Dado_Cuando_Entonces)

        [Fact]
        public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
        {

            // ------------ Arrange ------------ 
            var userRepo = A.Fake<IUserRepository>();
            var roleRepo = A.Fake<IRoleRepository>();

            var authService = new AuthService(userRepo, roleRepo, getDefaultConfigJwtDto());

            // Credenciales de login
            string email = "exampletest123@gmail.com";
            string password = "Test123$";

            // Genera hash y salt reales usando PasswordUtils
            string salt = PasswordUtils.GenerateRandomSalt();
            string hashedPassword = PasswordUtils.HashPasswordWithSalt(password, salt);

            // Crea un usuario fake que devuelve el repositorio
            var user = getCreateUserTest(hashedPassword, salt);

            // Fake: el repositorio devuelve el usuario cuando se busca por email
            A.CallTo(() => userRepo.FindByEmail(email))
                .Returns(Task.FromResult<User?>(user));

            // Crea la solicitud de login
            var request = new LoginRequest(email, password);

            // ------------ Act ------------ 
            // Llama al método Login
            var response = await authService.Login(request);

            // ------------ Assert ------------ 
            response.Should().NotBeNull(); // Verifica que la respuesta no sea nula
            response.AccessToken.Should().NotBeNullOrEmpty(); // Verifica que el token de acceso no sea nulo o vacío

            // Verifica que el repositorio fue llamado correctamente
            A.CallTo(() => userRepo.FindByEmail(email))
                .MustHaveHappenedOnceExactly();

        }

        [Fact]
        public async Task Login_ShouldThrowException_WhenUserNotFound()
        {

            // ------------ Arrange ------------
            var userRepo = A.Fake<IUserRepository>();
            var roleRepo = A.Fake<IRoleRepository>();

            var authService = new AuthService(userRepo, roleRepo, getDefaultConfigJwtDto());

            string emailNotFound = "emailincorrect@gmail.com";
            string password = "Test123$";

            A.CallTo(() => userRepo.FindByEmail(emailNotFound))
                .Returns(Task.FromResult<User?>(null));

            var request = new LoginRequest(emailNotFound, password);

            // ------------ Act ------------
            var result = async () => await authService.Login(request);

            // ------------ Assert ------------
            await result.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Credenciales invalidas");

            A.CallTo(() => userRepo.FindByEmail(emailNotFound))
                .MustHaveHappenedOnceExactly();

        }

        [Fact]
        public async Task Login_ShouldThrowsException_WhenPasswordIsInvalid()
        {

            // ------------ Arrange ------------
            var userRepo = A.Fake<IUserRepository>();
            var roleRepo = A.Fake<IRoleRepository>();

            var authService = new AuthService(userRepo, roleRepo, getDefaultConfigJwtDto());

            var email = "exampletest123@gmail.com";
            var correctPassword = "Test123$";
            var incorrectPassword = "WrongPassword1!";

            var salt = PasswordUtils.GenerateRandomSalt();
            var hashedPassword = PasswordUtils.HashPasswordWithSalt(correctPassword, salt);

            var user = getCreateUserTest(hashedPassword, salt);

            A.CallTo(() => userRepo.FindByEmail(email))
                .Returns(Task.FromResult<User?>(user));

            var request = new LoginRequest(email, incorrectPassword);

            // ------------ Act ------------
            // el async () => ... es necesario para capturar la excepción correctamente
            // se usa regularmente en test con asserts negativos en conjunto con ThrowAsync
            var result = async () => await authService.Login(request);

            // ------------ Asserts ------------
            await result.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Credenciales invalidas");

            A.CallTo(() => userRepo.FindByEmail(email))
                .MustHaveHappenedOnceExactly();

        }

        [Fact]
        public async Task Register_ShouldCreateUser_WhenDataIsValid()
        {

            // ------------ Arrange ------------
            var userRepo = A.Fake<IUserRepository>();
            var roleRepo = A.Fake<IRoleRepository>();

            var authService = new AuthService(userRepo, roleRepo, getDefaultConfigJwtDto());

            var request = new RegisterRequest(12345678, "exampletest123@gmail.com", "Test123$", "Test", "Test");

            // Fakes: no existen usuarios con el mismo email o DNI
            A.CallTo(() => userRepo.FindByEmail(request.Email))
                .Returns(Task.FromResult<User?>(null));

            A.CallTo(() => userRepo.FindByDni(request.Dni))
                .Returns(Task.FromResult<User?>(null));

            // Fake: el rol por defecto "PRESUPUESTISTA" existe
            var role = new Role()
            {
                Id = 1,
                Name = "PRESUPUESTISTA",
                RolePermissions = Enumerable.Empty<RolePermission>().ToList(),
                Users = Enumerable.Empty<User>().ToList()
            };

            A.CallTo(() => roleRepo.FindByName("PRESUPUESTISTA"))
                .Returns(Task.FromResult<Role?>(role));

            // ------------ Act ------------
            var result = await authService.Register(request);

            // ------------ Asserts ------------
            result.Should().NotBeNull();

            // Verifica que el usuario fue guardado con los datos correctos
            A.CallTo(() => userRepo.Save(A<User>.That.Matches(u =>
                u.Email == request.Email && // debe guardar el email que pidio el usuario
                u.Dni == request.Dni &&
                u.FirstName == request.FirstName &&
                u.LastName == request.LastName &&
                u.Role != null &&
                u.Role.Name == "PRESUPUESTISTA" &&
                !string.IsNullOrEmpty(u.Hash) &&
                !string.IsNullOrEmpty(u.Salt)
            ))).MustHaveHappenedOnceExactly();

        }

        [Fact]
        public async Task Register_ShouldThrowException_WhenEmailAlreadyExists()
        {

            // ------------ Arrange ------------
            var userRepo = A.Fake<IUserRepository>();
            var roleRepo = A.Fake<IRoleRepository>();

            var authService = new AuthService(userRepo, roleRepo, getDefaultConfigJwtDto());

            var request = new RegisterRequest(12345678, "exampletest1234@gmail.com", "Test123$", "Test", "Test");

            // Simula que ya existe un usuario con el email dado
            var existingUser = new User { Email = request.Email };

            // Fake: el repositorio devuelve el usuario existente cuando se busca por email
            A.CallTo(() => userRepo.FindByEmail(request.Email))
                .Returns(Task.FromResult<User?>(existingUser)); // Simula que el usuario ya existe

            // ------------ Act ------------
            var result = async () => await authService.Register(request);

            // ------------ Asserts ------------
            await result.Should().ThrowAsync<UserNotFoundException>()
                .WithMessage($"Error: Correo electronico '{request.Email}' ya existe");

            // Verifica que no se haya intentado guardar un nuevo usuario
            A.CallTo(() => userRepo.Save(A<User>._))
                .MustNotHaveHappened();

        }

        [Fact]
        public async Task Register_ShouldThrowException_WhenDniAlreadyExists()
        {

            // ------------ Arrange ------------
            // Configuración de fakes y del servicio
            var userRepo = A.Fake<IUserRepository>();
            var roleRepo = A.Fake<IRoleRepository>();

            // Instancia del servicio bajo prueba
            var authService = new AuthService(userRepo, roleRepo, getDefaultConfigJwtDto());

            // Datos de registro
            var request = new RegisterRequest(12345678, "exampletest123@gmail.com", "Test123$", "Test", "Test");

            // Simula que ya existe un usuario con el DNI dado
            var existingUser = new User { Dni = request.Dni };

            // Fake: el repositorio devuelve null para el email (no existe usuario con ese email)
            A.CallTo(() => userRepo.FindByEmail(request.Email))
                .Returns(Task.FromResult<User?>(null)); // No existe usuario con ese email

            // Fake: el repositorio devuelve el usuario existente cuando se busca por DNI
            A.CallTo(() => userRepo.FindByDni(request.Dni))
                .Returns(Task.FromResult<User?>(existingUser)); // Simula que el DNI ya existe

            // ------------ Act -------------
            // ejecución del método bajo prueba
            // el async () => ... es necesario para capturar la excepción correctamente
            var result = async () => await authService.Register(request);

            // ------------ Asserts ------------
            // Verifica que se lance la excepción correcta
            await result.Should().ThrowAsync<UserNotFoundException>()
                .WithMessage($"Error: Numero de DNI '{request.Dni}' ya existe");

            // Verifica que no se haya intentado guardar un nuevo usuario
            A.CallTo(() => userRepo.Save(A<User>._))
                .MustNotHaveHappened();

        }

        [Fact]
        public async Task Register_ShouldThrowException_WhenDefaultRoleNotFound()
        {

            // ------------ Arrange ------------
            var userRepo = A.Fake<IUserRepository>();
            var roleRepo = A.Fake<IRoleRepository>();

            var authService = new AuthService(userRepo, roleRepo, getDefaultConfigJwtDto());

            var request = new RegisterRequest(12345678, "exampletest123@gmail.com", "Test123$", "Test", "Test");

            // Fake: no existen usuarios con el mismo email o DNI
            A.CallTo(() => userRepo.FindByEmail(request.Email))
                .Returns(Task.FromResult<User?>(null));

            A.CallTo(() => userRepo.FindByDni(request.Dni))
                .Returns(Task.FromResult<User?>(null));

            // Fake: el rol por defecto "PRESUPUESTISTA" NO existe
            A.CallTo(() => roleRepo.FindByName("PRESUPUESTISTA"))
                .Returns(Task.FromResult<Role?>(null));

            // ------------ Act ------------
            var result = async () => await authService.Register(request);

            // ------------ Asserts ------------
            await result.Should().ThrowAsync<RoleNotFoundException>()
                .WithMessage($"Rol 'PRESUPUESTISTA' no existe.");

            // Verifica que no se haya intentado guardar un nuevo usuario
            A.CallTo(() => userRepo.Save(A<User>._))
                .MustNotHaveHappened();

        }

        #region
        private User getCreateUserTest(string hash, string salt)
        {
            return new User
            {
                Id= User.GenerateId(),
                Email = "exampletest123@gmail.com",
                Hash = hash,
                Salt = salt,
                Dni = 12345678,
                FirstName = "Test",
                LastName = "Test",
                Status = Status.ACTIVE,
                Role = new Role("ADMIN", Enumerable.Empty<string>())
            };
        }
        #endregion

        #region
        private JwtConfigDto getDefaultConfigJwtDto()
        {
            return new JwtConfigDto
            {
                Secret = "TEST_SECRET_123456789_TEST_SECRET_123456789",
                Audience = "TEST_AUDIENCE",
                Issuer = "TEST_ISSUER",
                ExpirationMinutes = 30
            };
        }
        #endregion

    }
}
