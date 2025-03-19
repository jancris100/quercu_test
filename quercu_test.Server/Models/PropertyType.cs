using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quercu_test.Server.Models
{
    public class PropertyType
    {
        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int Id { get; set; }

        [Required, MinLength(3)]
        public string Description { get; set; } = string.Empty;
    }
}
