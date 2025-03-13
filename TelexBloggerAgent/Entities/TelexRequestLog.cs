using System.ComponentModel.DataAnnotations;

namespace TelexBloggerAgent.Entities
{
    public class TelexRequestLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime RequestTime { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(255)]
        public string Endpoint { get; set; }

        public string RequestPayload { get; set; } // Store JSON data

        public string ResponsePayload { get; set; } // Store JSON data

        public int ResponseStatusCode { get; set; }
    }
}
