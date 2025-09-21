namespace org.apimiroc.core.shared.Dto.Request
{
    public class UpdateRolePermissionsRequest
    {
        public IEnumerable<string> AddPermissions { get; set; } = new List<string>();
        public IEnumerable<string> RemovePermissions { get; set; } = new List<string>();
    }
}
