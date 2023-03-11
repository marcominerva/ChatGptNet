using System.Text.Json.Serialization;
using ChatGptNet.Models.Converters;

namespace ChatGptNet.Models;

/// <summary>
/// Represents a chat completion response.
/// </summary>
public class ChatGptResponse
{
    /// <summary>
    /// Gets or sets the Id of the response
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the source object for this response.
    /// </summary>
    public string Object { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Conversation Id, that is used to group messages of the same conversation.
    /// </summary>
    public string ConversationId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the UTC date and time at which the response has been generated.
    /// </summary>
    [JsonPropertyName("created")]
    [JsonConverter(typeof(UnixToDateTimeConverter))]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets information about token usage.
    /// </summary>
    public ChatGptUsage Usage { get; set; } = new();

    /// <summary>
    /// Gets or sets the error occurred during the chat completion execution, if any.
    /// </summary>
    public ChatGptError? Error { get; set; }

    /// <summary>
    /// Gets or sets the list of choices that has been provided by chat completion.
    /// </summary>
    public ChatGptChoice[] Choices { get; set; } = Array.Empty<ChatGptChoice>();

    /// <summary>
    /// Gets a value that determines if the response was successful.
    /// </summary>
    public bool IsSuccessful => Error is null;

    /// <summary>
    /// Gets the content of the first choice, if available.
    /// </summary>
    /// <returns>The content of the first choice, if available</returns>
    public string? GetMessage() => Choices.FirstOrDefault()?.Message.Content.Trim();
}
