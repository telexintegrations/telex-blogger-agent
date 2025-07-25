using BloggerAgent.Application.Commons.A2aAgentSpec;
using BloggerAgent.Application.Contracts;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace BloggerAgent.Application.Helpers
{
    public class A2aAgent
    {
       
        public static string Get()
        {
            var agentA2A = new AgentCard()
            {
                Name = "Blogger Agent",
                Description = "Blogger Agent helps users generate high-quality blog content effortlessly using AI, providing structure, creativity, and SEO optimization.",
                Url = "https://telex-blogger-agent-qdp4.onrender.com/api/v1/blogger-agent",
                Version = "1.0.0",
                IconUrl = "https://res.cloudinary.com/dlu45noef/image/upload/e_gen_remove:prompt_(background);multiple_true/v1742882213/blogger-agent_lbfdiz.jpg",
                DocumentationUrl = "https://telex-blogger-agent-docs.onrender.com",
                Capabilities = new Capability
                {
                    Streaming = false,
                    PushNotifications = true,
                },
                DefaultInputModes = new[] { "application/json" },
                DefaultOutputModes = new[] { "application/json" },
                Provider = new AgentProvider()
                {
                    Organization = "AI Blogger Services Org",
                    Url = "https://telex-blogger-agent-qdp4.onrender.com/api/v1/blogger-agent"
                },                   
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

        public ISecurityScheme HandleSecurityScheme(ISecurityScheme scheme)
        {
            switch (scheme.Type)
            {
                case "apiKey":
                    return scheme as APIKeySecurityScheme;
                    //Console.WriteLine(api.Name);
                    break;
                case "oauth2":
                    return scheme as OAuth2SecurityScheme;
                    //Console.WriteLine(oauth.Flows.Count);
                    break;
                default:
                    return scheme as OpenIdConnectSecurityScheme;
            }
        }
    }
}
