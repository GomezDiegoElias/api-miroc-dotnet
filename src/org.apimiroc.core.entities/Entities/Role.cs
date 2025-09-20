using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace org.apimiroc.core.entities.Entities
{
    [Table("tbl_role")]
    public class Role
    {

        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("name")]
        public string Name { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public ICollection<User> Users { get; set; } = new List<User>();

        public Role() { }

        public Role(string name, IEnumerable<string> permissions)
        {
            Name = name;
            RolePermissions = permissions
                .Select(p => new RolePermission { Permission = new Permission { Name = p } })
                .ToList();
        }

        public static Role ADMIN => new Role("ADMIN", new[] { "CREATE", "READ", "UPDATE", "DELETE" });
        public static Role PRESUPUESTISTA => new Role("PRESUPUESTISTA", new[] { "CREATE_ORDER", "READ_ORDER" });
        public static Role USER => new Role("USER", new[] { "READ_ORDER" });


    }
}
