using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TelexBloggerAgent.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null;

        public string Name { get; set; } = null;

        public string Email { get; set; } = null;

        public string CompanyId { get; set; } = null; // Reference to Company
    }
}
