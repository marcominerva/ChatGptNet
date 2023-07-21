using System.Text.Json.Serialization;

namespace ChatGptNet.Models;

/// <summary>
/// Contains further details about the error.
/// </summary>
/// <seealso cref="ChatGptError"/>"/>
public class ChatGptInnerError
{
    /// <summary>
    /// Gets or sets the error code.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content filter results related to the error.
    /// </summary>
    [JsonPropertyName("content_filter_result")]
    public ChatGptContentFilterResults? ContentFilterResults { get; set; }
}