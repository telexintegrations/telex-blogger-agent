using System.Text.Json;

namespace BloggerAgent.Domain.Commons
{
    public class AgentSpec
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Url { get; set; } = "";
        public string Version { get; set; } = "";
        public Capability Capabilities { get; set; }
        public string[] DefaultInputModes { get; set; }
        public string[] DefaultOutputModes { get; set; }
        public Skill[] Skills { get; set; }

        public class Capability
        {
            public bool Streaming { get; set; }
            public bool PushNotifications { get; set; }
        }

        public class Skill
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string[] Tags { get; set; }
            public string[] Examples { get; set; }
        }

        public static string GetAgentCard()
        {
            var agentA2A = new AgentSpec()
            {
                Name = "AI Blogger Agent",
                Description = "Blogger Agent helps users generate high-quality blog content effortlessly using AI, providing structure, creativity, and SEO optimization.",
                Url = "https://telex-blogger-agent-qdp4.onrender.com/api/v1/blogger-agent/generate-blog",
                Version = "1.0.0",
                Capabilities = new Capability
                {
                    Streaming = false,
                    PushNotifications = false,
                },
                DefaultInputModes = new[] { "application/json" },
                DefaultOutputModes = new[] { "application/json" },
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


        
      /*  {
            "name": "AI Blogger Agent",
            "description": "Blogger Agent helps users generate high-quality blog content effortlessly using AI, providing structure, creativity, and SEO optimization.",
            "url": "https://telex-blogger-agent-qdp4.onrender.com/api/v1/blogger-agent/generate-blog",
            "version": "1.0.0",
            "capabilities": {
                "streaming": false,
                "pushNotifications": false
            },
            "defaultInputModes": [
                "application/json"
            ],
            "defaultOutputModes": [
                "application/json"
            ],
            "skills": [
                {
                    "id": "generate-blog",
                    "name": "Generate Blog Content",
                    "description": "Generates high-quality blog content using AI.",
                    "tags": [
                        "blog",
                        "content generation",
                        "AI"
                    ],
                    "examples": [
                        "Provides a chat-based Interaction with the AI",
                        "Generates high-quality blog content using AI in seconds.",
                        "Allows customization of tone and writing style.",
                        "Supports keyword-based content generation for SEO optimization.",
                        "Can create full articles, outlines, or summaries based on user input."
                    ]
                }
            ]
        }*/
    }
}
