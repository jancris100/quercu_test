using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quercu_test.Server.Models
{
    public class Property
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int PropertyTypeId { get; set; }

        [Required]
        public int OwnerId { get; set; }

        [Required, MinLength(1)] 
        public string Number { get; set; } = string.Empty;

        [Required, MinLength(5)]
        public string Address { get; set; } = string.Empty;

        [Required, Range(0.1, double.MaxValue)]
        public decimal Area { get; set; }

        public decimal? ConstructionArea { get; set; }

        public required Owner Owner { get; set; }

        public required PropertyType PropertyType { get; set; }
    }
}
