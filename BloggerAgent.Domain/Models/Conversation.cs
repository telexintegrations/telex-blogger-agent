using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using BloggerAgent.Domain.IRepositories;

namespace BloggerAgent.Domain.Models
{
    public class Conversation : IEntity
    {        
        public string Id { get; set; }
        public string UserId { get; set; }// Who started the conversation

        public List<Message> Messages { get; set; } 

        public string? ThreadId { get; set; } // If it's a reply, it links to a parent conversation

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
