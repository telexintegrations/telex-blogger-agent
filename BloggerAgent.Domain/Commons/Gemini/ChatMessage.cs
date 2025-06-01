using System.Text.Json.Serialization;

namespace BloggerAgent.Domain.Commons.Gemini
{
    public class ChatMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("parts")]
        public List<Part> Parts { get; set; } = new();
    }
}
