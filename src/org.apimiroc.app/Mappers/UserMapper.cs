using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.entities.Enums;
using org.apimiroc.core.shared.Dto.Request;
using org.apimiroc.core.shared.Dto.Response;
using org.apimiroc.core.shared.Utils;

namespace org.apimiroc.app.Mappers
{
    public static class UserMapper
    {

        public static UserResponse ToResponse(User user)
        {
            return new UserResponse(
                    user.Dni,
                    user.Email,
                    user.FirstName,
                    user.LastName ?? string.Empty,
                    user.Role.Name ?? string.Empty,
                    user.Status.ToString()
                );
        }

        public static UserUpdateRequest ToRequest(User user)
        {
            return new UserUpdateRequest(
                user.Dni,
                user.Email,
                string.Empty, // Nunca expone el password por seguridad
                user.FirstName,
                user.LastName,
                user.Status.ToString(),
                user.Role?.Name ?? string.Empty
            );
        }

        // Nuevo mapper especifico para PATCH que preserva valores existentes
        public static User ToEntityForPatch(UserUpdateRequest request, User existingUser)
        {
            var user = new User
            {
                Id = existingUser.Id, // Preservar ID
                Dni = request.Dni,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = new Role(request.Role, Enumerable.Empty<string>()),
                Status = Enum.Parse<Status>(request.Status, true),
                // Preserva el salt y hash existentes por defecto
                Salt = existingUser.Salt,
                Hash = existingUser.Hash
            };

            // Solo regenera el hash/salt si se mando un nuevo password
            if (!string.IsNullOrEmpty(request.Password))
            {
                var salt = PasswordUtils.GenerateRandomSalt();
                var hash = PasswordUtils.HashPasswordWithSalt(request.Password, salt);
                user.Salt = salt;
                user.Hash = hash;
            }

            return user;
        }

        // Mapper específico para PUT (actualización completa)
        public static User ToEntityForUpdate(UserUpdateRequest request, User existingUser)
        {
            var user = new User
            {
                Id = existingUser.Id, // Preservar ID
                Dni = request.Dni,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = new Role(request.Role, Enumerable.Empty<string>()),
                Status = Enum.Parse<Status>(request.Status, true),
                Salt = existingUser.Salt,     // Preservar por defecto
                Hash = existingUser.Hash      // Preservar por defecto
            };

            // Actualizar password solo si se proporciona
            if (!string.IsNullOrEmpty(request.Password))
            {
                var salt = PasswordUtils.GenerateRandomSalt();
                var hash = PasswordUtils.HashPasswordWithSalt(request.Password, salt);
                user.Salt = salt;
                user.Hash = hash;
            }

            return user;
        }

    }
}
