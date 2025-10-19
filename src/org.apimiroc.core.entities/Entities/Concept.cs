using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace org.apimiroc.core.entities.Entities
{
    [Table("tbl_concept")]
    public class Concept
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; }

        [Required]
        [Column("type")]
        public string type { get; set; } // "ingreso" | "egreso"

        [Column("description")]
        [MaxLength(250)]
        public string Description { get; set; }

        [InverseProperty("Concept")]
        public ICollection<Movement> Movements { get; set; } = new List<Movement>();

    }
}
