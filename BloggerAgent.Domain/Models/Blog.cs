using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using BloggerAgent.Domain.IRepositories;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization;
using BloggerAgent.Domain.DomainHelper;

namespace BloggerAgent.Domain.Models
{
    public class Blog : BaseEntity, IEntity
    {

        public string? Title { get; set; }

        public string? Content { get; set; }

        public List<string> Keywords { get; set; } = new();

    }
}
