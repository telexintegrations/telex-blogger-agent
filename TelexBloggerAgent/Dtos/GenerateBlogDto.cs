using System.Text.Json.Serialization;

namespace TelexBloggerAgent.Dtos
{
    public class GenerateBlogDto
    {
        [JsonPropertyName("channel_id")]
        public string ChannelId { get; set; }
        public string Message { get; set; }
        public List<Setting> Settings { get; set; }
    }
}
