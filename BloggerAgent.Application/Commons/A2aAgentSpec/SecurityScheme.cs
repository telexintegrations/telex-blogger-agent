using BloggerAgent.Application.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloggerAgent.Application.Commons.A2aAgentSpec
{
     
    public class OpenIdConnectSecurityScheme : ISecurityScheme
    {
        public string Type { get; set; } = "openId";
        public string OpenIdConnectUrl { get; set; }
    }

    public class APIKeySecurityScheme : ISecurityScheme
    {
        public string Type { get; set; } = "apiKey";
        public string Name { get; set; }        // Name of the header/query/cookie
        public string In { get; set; }          // "query", "header", "cookie"
    }

    public class OAuth2SecurityScheme : ISecurityScheme
    {
        public string Type { get; set; } = "oauth2";
        public Dictionary<string, OAuthFlow> Flows { get; set; }
    }

    public class OAuthFlow
    {
        public string AuthorizationUrl { get; set; }
        public string TokenUrl { get; set; }
        public string RefreshUrl { get; set; }
        public Dictionary<string, string> Scopes { get; set; }
    }

    public class HTTPAuthSecurityScheme : ISecurityScheme
    {
        public string Type { get; set; } = "jwt";
        public string Scheme { get; set; }          // e.g. "basic", "bearer"
        public string BearerFormat { get; set; }    // e.g. "JWT" (optional)
    }
}
