using org.apimiroc.core.entities.Entities;
using org.apimiroc.core.shared.Dto.Response;

namespace org.apimiroc.app.Mappers
{
    public static class RoleMapper
    {
        public static RoleResponse ToResponse(Role entity, bool includePermissions = true)
        {
            return new RoleResponse(
                entity.Name,
                includePermissions
                    ? entity.RolePermissions.Select(rp => rp.Permission.Name).ToList()
                    : new List<string>()
            );
        }
    }
}
