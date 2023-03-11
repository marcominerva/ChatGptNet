namespace ChatGptNet.Models;

/// <summary>
/// Represents a single chat message.
/// </summary>
public class ChatGptMessage
{
    /// <summary>
    /// Gets or sets the role (source) of the message. Valid values are: <em>system</em>, <em>user</em> and <em>assistant</em>.
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// The content of the message.
    /// </summary>
    public string Content { get; set; } = string.Empty;
}
