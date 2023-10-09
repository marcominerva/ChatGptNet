using System.Diagnostics.CodeAnalysis;

namespace ChatGptNet.Models.Common;

/// <summary>
/// Contains common properties for all response types.
/// </summary>
public class Response
{
    /// <summary>
    /// Gets or sets the source object for this response.
    /// </summary>
    public string Object { get; set; } = string.Empty;

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
    /// Gets a value that determines if the response was successful.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccessful => Error is null;
}