using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BloggerAgent.Domain.Commons.DataEntities
{
    public class GroqChatRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = "compound-beta";

        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; } = new();

        public class Message
        {
            [JsonPropertyName("role")]
            public string Role { get; set; } = "user";
            [JsonPropertyName("content")]
            public string Content { get; set; } = "";
        }
    }

}
