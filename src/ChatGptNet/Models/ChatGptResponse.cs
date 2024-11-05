using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using ChatGptNet.Extensions;
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
    /// This fingerprint represents the backend configuration that the model runs with.
    /// Can be used in conjunction with the <see cref="ChatGptParameters.Seed"/> request parameter to understand when backend changes have been made that might impact determinism.
    /// </summary>
    [JsonPropertyName("system_fingerprint")]
    public string? SystemFingerprint { get; set; }

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
    /// Gets a value indicating whether the first choice, if available, has been filtered by the content filtering system.
    /// </summary>
    /// <seealso cref="ChatGptChoice"/>
    /// <seealso cref="ChatGptChoice.IsFiltered"/>
    public bool IsContentFiltered => Choices.FirstOrDefault()?.IsFiltered ?? false;

    /// <summary>
    /// Gets the content of the response.
    /// </summary>
    /// <returns>The content of the response.</returns>
    public override string? ToString()
        => this.GetContent();
}