using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using ChatGptNet.Models.Converters;

namespace ChatGptNet.Models;

/// <summary>
/// Represents a chat completion response.
/// </summary>
public class ChatGptResponse
{
    /// <summary>
    /// Gets or sets the Id of the response.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the source object for this response.
    /// </summary>
    public string Object { get; set; } = string.Empty;

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
    /// Gets or sets the model name that has been used to generate the response.
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets information about token usage.
    /// </summary>
    /// <remarks>
    /// The <see cref="Usage"/> property is always <see langword="null"/> when requesting response streaming with <see cref="ChatGptClient.AskStreamAsync(Guid, string, ChatGptParameters?, string?, bool, CancellationToken)"/>.
    /// </remarks>
    public ChatGptUsage? Usage { get; set; }

    /// <summary>
    /// Gets or sets the error occurred during the chat completion execution, if any.
    /// </summary>
    public ChatGptError? Error { get; set; }

    /// <summary>
    /// Gets or sets the list of choices that has been provided by chat completion.
    /// </summary>
    public IEnumerable<ChatGptChoice> Choices { get; set; } = Enumerable.Empty<ChatGptChoice>();

    /// <summary>
    /// Gets or sets the list of prompt annotations determined by the content filtering system.
    /// </summary>
    [JsonPropertyName("prompt_annotations")]
    public IEnumerable<ChatGptPromptAnnotations>? PromptAnnotations { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether any prompt has been filtered by content filtering system.
    /// </summary>
    [MemberNotNullWhen(true, nameof(PromptAnnotations))]
    public bool IsPromptFiltered => PromptAnnotations?.Any(
        p => p.ContentFilterResults.Hate.Filtered || p.ContentFilterResults.SelfHarm.Filtered || p.ContentFilterResults.Violence.Filtered
            || p.ContentFilterResults.Sexual.Filtered) ?? false;

    /// <summary>
    /// Gets a value that determines if the response was successful.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccessful => Error is null;

    /// <summary>
    /// Gets the content of the first choice, if available.
    /// </summary>
    /// <returns>The content of the first choice, if available.</returns>
    /// <remarks>When using streaming responses, the <see cref="GetMessage"/> property returns a partial message delta.</remarks>
    /// <seealso cref="ChatGptRequest.Stream"/>
    public string? GetMessage() => Choices.FirstOrDefault()?.Delta?.Content ?? Choices.FirstOrDefault()?.Message.Content?.Trim();

    /// <summary>
    /// Gets a value indicating whether the first choice, if available, contains a function call. 
    /// </summary>
    public bool IsFunctionCall => Choices.FirstOrDefault()?.IsFunctionCall ?? false;

    /// <summary>
    /// Gets or sets the function call for the message of the first choice, if available.
    /// </summary>
    public ChatGptFunctionCall? GetFunctionCall() => Choices.FirstOrDefault()?.Message.FunctionCall;
}