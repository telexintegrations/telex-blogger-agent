using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TelexBloggerAgent.Models
{
    public class Blog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        public string UserId { get; set; } = null!; // Reference to the user who created the blog

        public string CompanyId { get; set; } = null!; // Reference to the company

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public List<string> Keywords { get; set; } = new(); // Keywords for SEO or categorization

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
