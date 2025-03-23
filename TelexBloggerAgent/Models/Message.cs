using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using TelexBloggerAgent.IRepositories;

namespace TelexBloggerAgent.Models
{
    public class Message : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = Guid.NewGuid().ToString();  // Unique per message
        public string ConversationId { get; set; }  // Format: "{CompanyId}_{UserId}"
        public string Role { get; set; } = null!; // "user" or "ai"
        public string Content { get; set; } = null;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
