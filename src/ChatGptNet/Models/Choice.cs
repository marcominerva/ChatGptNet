using System.Text.Json.Serialization;

namespace ChatGptNet.Models;

public class Choice
{
    public int Index { get; set; }

    public Message Message { get; set; } = new();

    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; set; } = string.Empty;
}
