using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using BloggerAgent.Domain.IRepositories;
using System.Text.Json.Serialization;

namespace BloggerAgent.Domain.Models
{
    public class Message : MessageBase, IEntity
    {
        [JsonPropertyName("is_channel_conversation")]
        public bool IsChannelConversation { get; set; } = false;
        public string ContextId { get; set; }
        public string TaskId { get; set; }
        public string Role { get; set; } = null;
        public string Content { get; set; } = null;
    }
}
