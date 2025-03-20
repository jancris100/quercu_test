using System.ComponentModel.DataAnnotations;

namespace quercu_test.Server.DTOS
{
    public class PropertyCreateDto
    {
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
    }
}
