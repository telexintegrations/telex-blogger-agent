using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json.Serialization;

namespace BloggerAgent.Infrastructure.Tooling.Types;

public class ToolDescriptor
{
    public string Name { get; set; }
    public string Description { get; set; }
    public ToolParameterSchema Parameters { get; set; }
}

public class ToolParameterSchema
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "object";

    [JsonPropertyName("properties")]
    public Dictionary<string, ToolParameter> Properties { get; set; }

    [JsonPropertyName("required")]
    public List<string> Required { get; set; }
}

public class ToolParameter
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }
}

public class ToolArrayParameter<T>
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("items")]
    public ArrayItems<T> Items { get; set; }
}

public class ArrayItems<T>
{
    public string Type { get; set; } = typeof(T).Name;
}