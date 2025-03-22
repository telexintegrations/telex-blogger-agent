using System.Text.Json.Serialization;

namespace TelexBloggerAgent.Dtos.GeminiDto
{
    public class ChatMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("parts")]
        public List<Part> Parts { get; set; } = new();
    }
}
