using System.Text.Json.Serialization;

namespace BloggerAgent.Domain.Commons.Gemini
{
    public class GeminiRequest
    {
        [JsonPropertyName("system_instruction")]
        public SystemMessage SystemInstruction { get; set; }

        [JsonPropertyName("contents")]
        public List<ChatMessage> Contents { get; set; } = new();
    }
}
