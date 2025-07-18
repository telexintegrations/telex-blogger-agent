using BloggerAgent.Domain.DomainHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BloggerAgent.Domain.Models
{
    public class MessageBase
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("_id")]
        public string? Id { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("organisation_id")]
        public string? OrganizationId { get; set; }

        [JsonPropertyName("tag")]
        public string Tag { get; set; }
        
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    }
}
