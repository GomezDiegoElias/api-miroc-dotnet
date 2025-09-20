using org.apimiroc.app.Dto.Response;
using org.apimiroc.core.entities.Entities;

namespace org.apimiroc.app.Mappers
{
    public static class UserMapper
    {

        public static UserResponse ToResponse(User user)
        {
            return new UserResponse(
                    user.Id,
                    user.Dni,
                    user.Email,
                    user.FirstName,
                    user.LastName ?? string.Empty,
                    user.Role.Name ?? string.Empty,
                    user.Status.ToString()
                );
        }

    }
}
