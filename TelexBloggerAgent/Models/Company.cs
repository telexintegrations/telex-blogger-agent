using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using TelexBloggerAgent.IRepositories;

namespace TelexBloggerAgent.Models
{
    public class Company : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null;

        public string Name { get; set; } = null;

        public string Overview { get; set; } = null;

        public string Industry { get; set; } = null;

        public string Website { get; set; } = null;

        public string Tone { get; set; } = null;

        public List<string> Users { get; set; } = new(); // List of user IDs

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; }
    }
}
