using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace org.apimiroc.core.entities.Entities
{
    [Table("tbl_provider")]
    public class Provider
    {

        [Key]
        [Column("id")]
        public string Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("first_name")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(300)]
        [Column("description")]
        public string Description { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("address")]
        public string Address { get; set; }

        [Required]
        [Column("cuit")]
        public long Cuit { get; set; }

        [Required]
        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") != null ? DateTime.Now : DateTime.MinValue;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") != null ? DateTime.Now : DateTime.MinValue;

        [InverseProperty(nameof(Movement.Provider))]
        public ICollection<Movement> Movements { get; set; } = new List<Movement>();

        public Provider() { }

        public Provider(string id, long cuit, string firstname, string address, string description)
        {
            Id = id;
            Cuit = cuit;
            FirstName = firstname;
            Address = address;
            Description = description;
            IsDeleted = false;
        }

        public static string GenerateId()
        {
            string timestamp = DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmss");
            string uuidPart = Guid.NewGuid().ToString().Split('-')[0];
            return $"cli-{timestamp}-{uuidPart}";
        }

    }
}






