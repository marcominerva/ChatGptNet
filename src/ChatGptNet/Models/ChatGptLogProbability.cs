namespace ChatGptNet.Models;

/// <summary>
/// Represents the log probability information of a <see cref="ChatGptChoice">completion choice</see>.
/// </summary>
public class ChatGptLogProbability
{
    /// <summary>
    /// Gets or sets the list of message content tokens with log probability information.
    /// </summary>
    public IEnumerable<ChatGptLogProbabilityContent> Content { get; set; } = [];
}