namespace TelexBloggerAgent.Dtos
{
    public class GenerateBlogDto
    {        
        public string Message { get; set; }
        public CompanyDto Company { get; set; }
        public List<Setting> Settings { get; set; }
    }
}
