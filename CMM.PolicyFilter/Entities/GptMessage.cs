using System.Text.Json.Serialization;

namespace CMM.PolicyFilter.Entities;

public class GptMessage
{
    [JsonPropertyName("role")] public string Role { get; set; }

    [JsonPropertyName("content")] public string Content { get; set; }
}