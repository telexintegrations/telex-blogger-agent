using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using BloggerAgent.Domain.IRepositories;

namespace BloggerAgent.Domain.Models
{
    public class Message : IEntity
    {
        
        public string Id { get; set; } 
        public string ContextId { get; set; }    
        public string TaskId { get; set; } 
        public string Role { get; set; } = null; 
        public string Content { get; set; } = null;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
