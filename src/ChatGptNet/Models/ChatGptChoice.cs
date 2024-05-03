using System.Diagnostics.CodeAnalysis;
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
    /// Gets or sets the message associated with this <see cref="ChatGptChoice"/>, if any.
    /// </summary>
    /// <seealso cref="ChatGptChoice"/>    
    public ChatGptMessage? Message { get; set; }

    /// <summary>
    /// Gets or sets the content filter results for the this <see cref="ChatGptChoice"/>.
    /// </summary>
    /// <seealso cref="ChatGptChoice"/>
    [JsonPropertyName("content_filter_results")]
    public ChatGptContentFilterResults? ContentFilterResults { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the this <see cref="ChatGptChoice"/> has been filtered by the content filtering system.
    /// </summary>
    /// <seealso cref="ChatGptChoice"/>
    [MemberNotNullWhen(true, nameof(ContentFilterResults))]
    public bool IsFiltered => ContentFilterResults is not null
        && (ContentFilterResults.Hate.Filtered || ContentFilterResults.SelfHarm.Filtered || ContentFilterResults.Violence.Filtered
            || ContentFilterResults.Sexual.Filtered);

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
    /// Gets or sets the log probabilities associated with this <see cref="ChatGptChoice"/>.
    /// </summary>
    /// <seealso cref="ChatGptLogProbability"/>
    /// <seealso cref="ChatGptLogProbabilityContent"/>
    [JsonPropertyName("logprobs")]
    public ChatGptLogProbability? LogProbabilities { get; set; }
}
