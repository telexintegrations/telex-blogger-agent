using System.Text.Json.Serialization;

namespace TelexBloggerAgent.Dtos.GeminiDto
{
    public class Part
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
