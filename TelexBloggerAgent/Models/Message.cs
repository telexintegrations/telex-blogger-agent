namespace TelexBloggerAgent.Models
{
    public class Message
    {
        public string Role { get; set; } = null!; // "user" or "ai"
        public string Content { get; set; } = null;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
