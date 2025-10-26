using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace org.apimiroc.core.entities.Entities
{
    [Table("tbl_movement")]
    public class Movement
    {

        [Key]
        [Column("id")]
        public string Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("cod_movement")]
        public int CodMovement { get; set; }

        [Required]
        [Column("amount")]
        public decimal Amount { get; set; }

        [Required]
        [Column("date")]
        public DateTime Date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") != null ? DateTime.Now : DateTime.Now;

        [Required]
        [Column("payment_method")]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CASH;

        // estado
        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;

        // Llave foránea a Concept
        [Required]
        [Column("concept_id")]
        public int ConceptId { get; set; }

        // Propiedad de navegación
        [ForeignKey(nameof(ConceptId))]
        public Concept Concept { get; set; }

        // Relaciones opcionales (FKs pueden ser null)
        [Column("client_id")]
        public string? ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        public Client? Client { get; set; }

        [Column("provider_id")]
        public string? ProviderId { get; set; }

        [ForeignKey(nameof(ProviderId))]
        public Provider? Provider { get; set; }

        [Column("employee_id")]
        public string? EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public Employee? Employee { get; set; }

        [Column("construction_id")]
        public string? ConstructionId { get; set; }

        [ForeignKey(nameof(ConstructionId))]
        public Construction? Construction { get; set; }

        public Movement() { }

        public Movement(string id, decimal amount, DateTime date, PaymentMethod paymentMethod, int conceptId)
        {
            Id = id;
            Amount = amount;
            Date = date;
            PaymentMethod = paymentMethod;
            ConceptId = conceptId;
        }

        public static string GenerateId()
        {
            string timestamp = DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmss");
            string uuidPart = Guid.NewGuid().ToString().Split('-')[0];
            return $"mov-{timestamp}-{uuidPart}";
        }

    }
}
