namespace TelexBloggerAgent.Dtos
{
    public class RefineBlogDto
    {
        public string? Message { get; set; } // The content to refine
        public string RefinementInstructions { get; set; } // Instructions for refinement
        public List<Setting> Settings { get; set; } // Settings for the blog (webhook URL)
    }
}
