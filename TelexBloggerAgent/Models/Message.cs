using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using TelexBloggerAgent.IRepositories;

namespace TelexBloggerAgent.Models
{
    public class Message : IEntity
    {
        
        public string Id { get; set; } // Unique per message
        public string SourceId { get; set; }    
        public string ConversationId { get; set; }  // Format: "{CompanyId}_{UserId}"
        public string Role { get; set; } = null!; // "user" or "ai"
        public string Content { get; set; } = null;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
