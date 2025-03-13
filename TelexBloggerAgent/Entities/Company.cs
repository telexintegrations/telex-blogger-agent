using System.ComponentModel.DataAnnotations;

namespace TelexBloggerAgent.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(255)]
        public string Website { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
