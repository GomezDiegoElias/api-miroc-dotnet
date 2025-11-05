using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace org.apimiroc.core.entities.Entities
{
    [Table("tbl_construction")]
    public class Construction
    {

        [Key]
        [Column("id")]
        public string Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("name")]
        public string Name { get; set; }

        [Required]
        [Column("startDate")]
        public DateTime StartDate { get; set; } 

        [Column("endDate")]
        public DateTime EndDate { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("address")]
        public string Address { get; set; }

        [Required]
        [MaxLength(300)]
        [Column("description")]
        public string Description { get; set; }

        [Required]
        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;

        [Required]
        [Column("create_at")]
        public DateTime CreateAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") != null ? DateTime.Now : DateTime.Now;

        [Required]
        [Column("update_at")]
        public DateTime UpdateAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") != null ? DateTime.Now : DateTime.Now;

        [Required]
        [Column("cient_id")]
        public string ClientId { get; set; }

        // Propiedad de navegación
        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; }

        [InverseProperty(nameof(Movement.Construction))]
        public ICollection<Movement> Movements { get; set; } = new List<Movement>();

        public Construction() { }

        public Construction(string id, string name, DateTime startDate, DateTime endDate, string address, string description, string clientId)
        {
            Id = id;
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            Address = address;
            Description = description;
            ClientId = clientId;
        }

        public static string GenerateId()
        {
            string timestamp = DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmss");
            string uuidPart = Guid.NewGuid().ToString().Split('-')[0];
            return $"obr-{timestamp}-{uuidPart}";
        }

    }
}
