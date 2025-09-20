using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace org.apimiroc.core.entities.Entities
{
    [Table("tbl_role_permission")]
    public class RolePermission
    {

        [Key]
        [Column("id")]
        public int Id { get; set; }

        [ForeignKey("Role")]
        [Column("role_id")]
        public int RoleId { get; set; }
        public Role Role { get; set; }

        [ForeignKey("Permission")]
        [Column("permission_id")]
        public int PermissionId { get; set; }
        public Permission Permission { get; set; }

    }
}
