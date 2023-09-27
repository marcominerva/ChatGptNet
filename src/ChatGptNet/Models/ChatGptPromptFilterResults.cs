using System.Text.Json.Serialization;

namespace ChatGptNet.Models;

/// <summary>
/// Contains information about content filtering for input prompts.
/// </summary>
/// <remarks>
/// See <see href="https://learn.microsoft.com/en-us/azure/ai-services/openai/concepts/content-filter">Content filtering</see> for more information.
/// </remarks>
public class ChatGptPromptFilterResults
{
    /// <summary>
    /// Gets or sets the index of the prompt to which the annotation refers.
    /// </summary>
    [JsonPropertyName("prompt_index")]
    public int Index { get; set; }

    /// <summary>
    /// Gets or sets the content filter results for the prompt.
    /// </summary>
    [JsonPropertyName("content_filter_results")]
    public ChatGptContentFilterResults ContentFilterResults { get; set; } = new();
}
