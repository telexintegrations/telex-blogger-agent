using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using BloggerAgent.Domain.IRepositories;

namespace BloggerAgent.Domain.Models
{
    public class User : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = null;
        //public string CompanyId { get; set; } = null; // Reference to Company

        //public string Name { get; set; } = null;

        //public string Email { get; set; } = null;

    }
}
