using System.Diagnostics.CodeAnalysis;

namespace ChatGptNet.Models;

/// <summary>
/// Represents an embedding response.
/// </summary>
public class ChatGptEmbeddingResponse
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
    /// Gets or sets the model name that has been used to generate the response.
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error occurred during the chat completion execution, if any.
    /// </summary>
    public ChatGptError? Error { get; set; }

    /// <summary>
    /// Gets or sets information about token usage.
    /// </summary>
    /// <remarks>
    /// The <see cref="Usage"/> property is always <see langword="null"/> when requesting response streaming with <see cref="ChatGptClient.AskStreamAsync(Guid, string, ChatGptParameters?, string?, bool, CancellationToken)"/>.
    /// </remarks>
    public ChatGptUsage? Usage { get; set; }

    /// <summary>
    /// Gets a value that determines if the response was successful.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccessful => Error is null;

    /// <summary>
    /// Array of Embedding objects created by ChatGpt
    /// </summary>
    public ChatGptEmbedding[] Data { get; set; } = Array.Empty<ChatGptEmbedding>();
}