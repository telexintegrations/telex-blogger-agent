using BloggerAgent.Domain.DomainHelper;
using System.Text.Json.Serialization;

namespace BloggerAgent.Domain.Models
{
    public class BaseEntity
    {
        [JsonPropertyName("_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Id { get; set; } 

        [JsonPropertyName("user_id")]
        public string? UserId { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("organisation_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? OrganizationId { get; set; }

        [JsonPropertyName("tag")]
        public string Tag { get; set; }
    }
}
