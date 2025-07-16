using BloggerAgent.Domain.DomainHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BloggerAgent.Domain.Models
{
    public class BaseEntity
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; } = IdGenerator.GenerateObjectId();
        [JsonPropertyName("user_id")]
        public string? UserId { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("organization_id")]
        public string? OrganizationId { get; set; }
        [JsonPropertyName("tag_name")]
        public string TagName { get; set; }
    }
}
