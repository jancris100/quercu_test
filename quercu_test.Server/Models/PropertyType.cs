using System.ComponentModel.DataAnnotations;

namespace quercu_test.Server.Models
{
    public class PropertyType
    {
        public int Id { get; set; }

        [Required, MinLength(3)]
        public string Description { get; set; } = string.Empty;
    }
}
