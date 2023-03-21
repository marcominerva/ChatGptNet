using System.Text.Json.Serialization;

namespace ChatGptNet.Models;

/// <summary>
/// Represents a request for a chat completions.
/// </summary>
public class ChatGptRequest : ChatGptParameters
{
    /// <summary>
    /// Gets or sets the ID of the model to use. Currently, only <em>gpt-3.5-turbo</em> and <em>gpt-3.5-turbo-0301</em> are supported.
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the messages to generate chat completions for.
    /// </summary>
    /// <seealso cref="ChatGptMessage"/>
    public ChatGptMessage[] Messages { get; set; } = Array.Empty<ChatGptMessage>();

    /// <summary>
    /// Gets or sets a value that specify if response will be sent in streaming as partial message deltas.
    /// </summary>
    public bool Stream { get; set; }
}
