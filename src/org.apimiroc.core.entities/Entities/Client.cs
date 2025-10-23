using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace org.apimiroc.core.entities.Entities
{
    [Table("tbl_client")]
    public class Client
    {

        [Key]
        [Column("id")]
        public string Id { get; set; }

        [Required]
        [Column("dni")]
        public long Dni { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("first_name")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("address")]
        public string Address { get; set; }

        [Required]
        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") != null ? DateTime.Now : DateTime.MinValue;

        [Required]
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") != null ? DateTime.Now : DateTime.MinValue;

        [InverseProperty(nameof(Movement.Client))]
        public ICollection<Movement> Movements { get; set; } = new List<Movement>();

        public Client() { }

        public Client(string id, long dni, string firstname, string address)
        {
            Id = id;
            Dni = dni;
            FirstName = firstname;
            Address = address;
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
