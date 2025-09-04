using BloggerAgent.Application.Commons;
using BloggerAgent.Application.Contracts;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace BloggerAgent.Application.Helpers
{
    public class AgentCard
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
        public Dictionary<string, ISecurityScheme> SecuritySchemes { get; set; }
        public bool SuppostsAuthenticatedExtendedCard { get; set; }

    }

    public class AgentProvider
    {
        public string Organization { get; set; } = "";
        public string Url { get; set; } = "";
    }
    
    public class Capability
    {
        public bool Streaming { get; set; }
        public bool PushNotifications { get; set; }
        public bool StateTransitionHistory { get; set; }
    }
    
    public class Skill
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string[] Tags { get; set; }
        public string[] Examples { get; set; }
    }
}
