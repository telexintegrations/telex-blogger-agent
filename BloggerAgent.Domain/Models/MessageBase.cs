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
        [JsonPropertyName("_id")]
        public string Id { get; set; } = IdGenerator.GenerateObjectId();

        [JsonPropertyName("organization_id")]
        public string OrganizationId { get; set; }

        [JsonPropertyName("tag_name")]
        public string TagName { get; set; }
        
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    }
}
