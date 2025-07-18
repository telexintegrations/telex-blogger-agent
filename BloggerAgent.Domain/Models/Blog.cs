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
        public string TaskId { get; set; } 
        public string Title { get; set; }
        public List<string> Keywords { get; set; }
        public string Outline { get; set; }
        public string ImageUrl { get; set; }
        public string Content { get; set; }
        public List<string> ReferenceLinks { get; set; } = new();
    }
    
}
