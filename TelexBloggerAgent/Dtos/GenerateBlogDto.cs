using System.Text.Json.Serialization;

namespace TelexBloggerAgent.Dtos
{
    public class GenerateBlogDto
    {
        
        [JsonPropertyName("message")]
        public string Message { get; set; }
        
        [JsonPropertyName("channel_id")]
        public string? ChannelId { get; set; }

        [JsonPropertyName("settings")]
        public List<Setting> Settings { get; set; }
    }
}
