using System.Text.Json.Serialization;

namespace TelexBloggerAgent.Dtos.GeminiDto
{
    public class SystemMessage
    {
        [JsonPropertyName("parts")]
        public Part Parts { get; set; } = new();
    }
}
