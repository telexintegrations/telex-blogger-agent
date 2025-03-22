using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using TelexBloggerAgent.IRepositories;

namespace TelexBloggerAgent.Models
{
    public class Conversation : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string? CompanyId { get; set; } = null; // Links the conversation to a company

        public string UserId { get; set; } = null!; // Who started the conversation

        public List<Message> Messages { get; set; } = null!;

        public string? ParentId { get; set; } // If it's a reply, it links to a parent conversation

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
