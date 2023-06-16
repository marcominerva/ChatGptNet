using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChatGptNet.Models;

/// <summary>
/// Represents a single chat message.
/// </summary>
public class ChatGptMessage
{
    /// <summary>
    /// Gets or sets the role (source) of the message.
    /// </summary>
    /// <remarks>
    ///  Valid values are: <em>system</em>, <em>user</em> and <em>assistant</em>.
    ///  </remarks>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// The content of the message.
    /// </summary>
    public string? Content { get; set; } = null;

    [JsonPropertyName("function_call")]
    public ChatGptFunctionCall? FunctionCall { get; set; } = null!;
}

public class ChatGptFunctionCall
{
    public string Name { get; set; } = string.Empty;

    public JsonDocument? Arguments { get; set; } = null!;
}
