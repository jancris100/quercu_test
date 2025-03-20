using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quercu_test.Server.Models
{
    public class Owner
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MinLength(3)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Telephone { get; set; } = string.Empty;

        [EmailAddress] 
        public string? Email { get; set; }

        [Required, MinLength(5)]
        public string IdentificationNumber { get; set; } = string.Empty;

        public string? Address { get; set; }

    }
}
