namespace TelexBloggerAgent.Dtos
{
    public class Setting
    {
        public string Label { get; set; }
        public string Type { get; set; }
        public string? Description { get; set; }
        public bool? Is_required { get; set; }
        public object Default { get; set; }
    }
}
