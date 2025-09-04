using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using BloggerAgent.Domain.IRepositories;
using System.Text.Json.Serialization;

namespace BloggerAgent.Domain.Models
{
    public class Company : BaseEntity, IEntity
    {        
        public string? Name { get; set; }

        public string? Overview { get; set; }

        public string? Industry { get; set; }

        public string? Website { get; set; }

        public string? Tone { get; set; }

        [JsonPropertyName("taget_audience")]
        public string? TargetAudience { get; set; }
    }
}
