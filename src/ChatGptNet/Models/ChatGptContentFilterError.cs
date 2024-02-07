namespace ChatGptNet.Models;

/// <summary>
/// Contains information about the error occurred in the content filtering system.
/// </summary>
/// <seealso cref="ChatGptContentFilterResults"/>
public class ChatGptContentFilterError
{
    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error code.
    /// </summary>
    public string? Code { get; set; }
}