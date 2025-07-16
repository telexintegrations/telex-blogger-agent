using BloggerAgent.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BloggerAgent.Domain.Models
{
    public class ApiKey : IEntity
    {
        [JsonPropertyName("_id")]
        public string? Id { get; set; }

        [JsonPropertyName("tag_name")]
        public string TagName { get; set; }

        [JsonPropertyName("organization_id")]
        public string? OrganizationId { get; set; }

        public string? Key { get; set; }
    }
}
