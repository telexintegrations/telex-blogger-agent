using System.Text.Json.Serialization;

namespace BloggerAgent.Domain.Commons.Gemini
{
    public class SystemMessage
    {
        [JsonPropertyName("parts")]
        public Part Parts { get; set; } = new();
    }
}
