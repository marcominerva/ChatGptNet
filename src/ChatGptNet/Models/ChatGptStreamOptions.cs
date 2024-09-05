using System.Text.Json.Serialization;

namespace ChatGptNet.Models;

internal class ChatGptStreamOptions
{
    [JsonPropertyName("include_usage")]
    public bool IncludeUsage { get; set; } = true;
}
