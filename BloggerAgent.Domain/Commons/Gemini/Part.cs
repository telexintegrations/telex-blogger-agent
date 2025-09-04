using System.Text.Json.Serialization;

namespace BloggerAgent.Domain.Commons.Gemini
{
    public class Part
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

    }
}
