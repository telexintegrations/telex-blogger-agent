using System.Text.Json.Serialization;

namespace BloggerAgent.Domain.Commons.DataEntities
{
    public class Document<T>
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("agent_id")]
        public string AgentId { get; set; }

        [JsonPropertyName("organisation_id")]
        public string OrganizationId { get; set; }

        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        [JsonPropertyName("data")]
        public T Data { get; set; }
    }
}
