using System.Text.Json.Serialization;

namespace ChatGptNet.Models;

/// <summary>
/// Represent a chat completion choice.
/// </summary>
public class ChatGptChoice
{
    /// <summary>
    /// Gets or sets the index of the choice in the list.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Gets or sets the message associated with this <see cref="ChatGptChoice"/>.
    /// </summary>
    /// <seealso cref="ChatGptChoice"/>
    public ChatGptMessage Message { get; set; } = new();

    /// <summary>
    /// Gets or sets a value specifying why the choice has been returned. Possible values are: <em>stop</em> (API returned complete model output), <em>length</em> (incomplete model output due to max_tokens parameter or token limit), <em>content_filter</em> (omitted content due to a flag from content filters) or <em>null</em> (API response still in progress or incomplete).
    /// </summary>
    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; set; } = string.Empty;
}
