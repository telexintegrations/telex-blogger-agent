using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TelexBloggerAgent.Models
{
    public class Company
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Industry { get; set; } = null!;

        public string Website { get; set; } = null!;

        public List<string> Users { get; set; } = new(); // List of user IDs
    }
}
