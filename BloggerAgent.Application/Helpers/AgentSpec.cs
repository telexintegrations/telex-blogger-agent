using BloggerAgent.Application.Commons;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace BloggerAgent.Application.Helpers
{
    public class AgentSpec
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Url { get; set; } = "";
        public string Version { get; set; } = "";
        public string IconUrl { get; set; } = "";
        public string DocumentationUrl { get; set; } = "";
        public Capability Capabilities { get; set; }
        public string[] DefaultInputModes { get; set; }
        public string[] DefaultOutputModes { get; set; }
        public Skill[] Skills { get; set; }
        public AgentProvider Provider { get; set; }
        public List<Dictionary<string, List<string>>> Security { get; set; }
        public Dictionary<string, SecurityScheme> SecuritySchemes { get; set; }
        public bool SuppostsAuthenticatedExtendedCard { get; set; }

       
        public static string GetAgentCard()
        {
            var agentA2A = new AgentSpec()
            {
                Name = "AI Blogger Agent",
                Description = "Blogger Agent helps users generate high-quality blog content effortlessly using AI, providing structure, creativity, and SEO optimization.",
                Url = "https://telex-blogger-agent-qdp4.onrender.com/api/v1/blogger-agent",
                Version = "1.0.0",
                IconUrl = "https://res.cloudinary.com/dlu45noef/image/upload/e_gen_remove:prompt_(background);multiple_true/v1742882213/blogger-agent_lbfdiz.jpg",
                DocumentationUrl = "https://telex-blogger-agent-docs.onrender.com",
                Capabilities = new Capability
                {
                    Streaming = false,
                    PushNotifications = false,
                },
                DefaultInputModes = new[] { "application/json" },
                DefaultOutputModes = new[] { "application/json" },
                Provider = new AgentProvider()
                {
                    Organization = "AI Blogger Services Org",
                    Url = "https://telex-blogger-agent-docs.onrender.com"
                },   
                //SecuritySchemes = new Dictionary<string, SecurityScheme>()
                //{
                //    ["bearerAuth"] = new HTTPAuthSecurityScheme()
                //    {
                //        Type = "openIdConnect",
                //        Scheme = "openIdConnect",
                //        BearerFormat = "JWT",

                //    },
                //},
                Skills = new[]
                {
                    new Skill
                    {
                        Id = "generate-blog",
                        Name= "Generate Blog Content",
                        Description= "Generates high-quality blog content using AI.",
                        Tags = [
                            "blog",
                            "content generation",
                            "AI"
                        ],
                        Examples = new[] {
                            "Provides a chat-based Interaction with the AI",
                            "Generates high-quality blog content using AI in seconds.",
                            "Allows customization of tone and writing style.",
                            "Supports keyword-based content generation for SEO optimization.",
                            "Can create full articles, outlines, or summaries based on user input."
                        }
                    }
                }
            };

            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Serialize(agentA2A, options);
        }
    }
}
