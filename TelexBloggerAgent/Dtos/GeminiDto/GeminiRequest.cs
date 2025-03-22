using System.Text.Json.Serialization;

namespace TelexBloggerAgent.Dtos.GeminiDto
{
    public class GeminiRequest
    {
        [JsonPropertyName("system_instruction")]
        public SystemMessage SystemInstruction { get; set; }

        [JsonPropertyName("contents")]
        public List<ChatMessage> Contents { get; set; } = new();
    }
}
