using System.Text.Json.Serialization;

namespace CMM.PolicyFilter.Entities;

public class GptResponse
{
    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("object")] public string Object { get; set; }

    [JsonPropertyName("created")] public long Created { get; set; }

    [JsonPropertyName("choices")] public List<GptResponseChoice> Choices { get; set; }

    [JsonPropertyName("usage")] public GptResponseUsageInfo UsageInfo { get; set; }

    [JsonPropertyName("model")] public string Model { get; set; }
}

public class GptResponseChoice
{
    [JsonPropertyName("index")] public int Index { get; set; }

    [JsonPropertyName("message")] public GptMessage Message { get; set; }

    [JsonPropertyName("finish_reason")] public string FinishReason { get; set; }
}

public class GptResponseUsageInfo
{
    [JsonPropertyName("prompt_tokens")] public int PromptTokens { get; set; }

    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; set; }

    [JsonPropertyName("total_tokens")] public int TotalTokens { get; set; }
}