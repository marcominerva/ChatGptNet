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
    /// When using streaming responses, gets or sets the partial message delta associated with this <see cref="ChatGptChoice"/>.
    /// </summary>
    /// <see cref="ChatGptRequest.Stream"/>
    public ChatGptMessage? Delta { get; set; }

    /// <summary>
    /// Gets or sets a value specifying why the choice has been returned.
    /// </summary>
    /// <remarks>
    /// Possible values are:
    /// <list type="bullet">
    /// <item><description>stop: API returned complete model output</description></item>
    /// <item><description>length: incomplete model output due to <em>max_tokens</em> parameter or token limit</description></item>
    /// <item><description>function_call: the model decided to call a function</description></item>
    /// <item><description>content_filter: omitted content due to a flag from content filters</description></item>
    /// <item><description>null: API response still in progress or incomplete</description></item>
    /// </list>
    /// </remarks>
    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether this choice contains a function call. 
    /// </summary>
    public bool IsFunctionCall => Message.FunctionCall is not null;
}
