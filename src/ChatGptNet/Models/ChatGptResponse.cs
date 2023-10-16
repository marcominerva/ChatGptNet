using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using ChatGptNet.Models.Common;
using ChatGptNet.Models.Converters;

namespace ChatGptNet.Models;

/// <summary>
/// Represents a chat completion response.
/// </summary>
public class ChatGptResponse : Response
{
    /// <summary>
    /// Gets or sets the Id of the response.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Conversation Id, that is used to group messages of the same conversation.
    /// </summary>
    public Guid ConversationId { get; set; }

    /// <summary>
    /// Gets or sets the UTC date and time at which the response has been generated.
    /// </summary>
    [JsonPropertyName("created")]
    [JsonConverter(typeof(UnixToDateTimeConverter))]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the list of choices that has been provided by chat completion.
    /// </summary>
    public IEnumerable<ChatGptChoice> Choices { get; set; } = Enumerable.Empty<ChatGptChoice>();

    /// <summary>
    /// Gets or sets the list of prompt filter results determined by the content filtering system.
    /// </summary>
    [JsonPropertyName("prompt_filter_results")]
    public IEnumerable<ChatGptPromptFilterResults>? PromptFilterResults { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether any prompt has been filtered by the content filtering system.
    /// </summary>
    [MemberNotNullWhen(true, nameof(PromptFilterResults))]
    public bool IsPromptFiltered => PromptFilterResults?.Any(
        p => p.ContentFilterResults.Hate.Filtered || p.ContentFilterResults.SelfHarm.Filtered || p.ContentFilterResults.Violence.Filtered
            || p.ContentFilterResults.Sexual.Filtered) ?? false;

    /// <summary>
    /// Gets the content of the first choice, if available.
    /// </summary>
    /// <returns>The content of the first choice, if available.</returns>
    /// <remarks>When using streaming responses, this method returns a partial message delta.</remarks>
    /// <seealso cref="ChatGptRequest.Stream"/>
    public string? GetContent() => Choices.FirstOrDefault()?.Delta?.Content ?? Choices.FirstOrDefault()?.Message?.Content?.Trim();

    /// <summary>
    /// Gets the content of the first choice, if available.
    /// </summary>
    /// <returns>The content of the first choice, if available.</returns>
    /// <remarks>When using streaming responses, this method returns a partial message delta.</remarks>
    /// <seealso cref="ChatGptRequest.Stream"/>
    [Obsolete("This method will be removed in the next version. Use GetContent() instead.")]
    public string? GetMessage() => GetContent();

    /// <summary>
    /// Gets a value indicating whether the first choice, if available, has been filtered by the content filtering system.
    /// </summary>
    /// <seealso cref="ChatGptChoice"/>
    /// <seealso cref="ChatGptChoice.IsFiltered"/>
    public bool IsContentFiltered => Choices.FirstOrDefault()?.IsFiltered ?? false;

    /// <summary>
    /// Gets a value indicating whether the first choice, if available, contains a function call. 
    /// </summary>
    /// <seealso cref="GetFunctionCall"/>
    /// <seealso cref="ChatGptFunctionCall"/>
    public bool IsFunctionCall => Choices.FirstOrDefault()?.IsFunctionCall ?? false;

    /// <summary>
    /// Gets or sets the function call for the message of the first choice, if available.
    /// </summary>
    public ChatGptFunctionCall? GetFunctionCall() => Choices.FirstOrDefault()?.Message?.FunctionCall;
}