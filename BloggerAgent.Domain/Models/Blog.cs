using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using BloggerAgent.Domain.IRepositories;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization;
using BloggerAgent.Domain.DomainHelper;
using BloggerAgent.Domain.Enums;

namespace BloggerAgent.Domain.Models
{
    public class Blog : BaseEntity, IEntity
    {
        public string ContextId { get; set; }
        public string TaskId { get; set; } 
        public string Title { get; set; }
        public List<string> Keywords { get; set; }
        public string Outline { get; set; }
        public string ImageUrl { get; set; }
        public string BlogContent { get; set; }
        public Status Status { get; set; }
        public TaskPhase CurrentPhase { get; set; }
        public List<TaskPhase> History { get; set; } = new();
        public List<string> ReferenceLinks { get; set; } = new();
    }
    
}
