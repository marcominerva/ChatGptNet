using System.Text.Json;

namespace ChatGptNet.Models;

/// <summary>
/// Represents a function call.
/// </summary>
public class ChatGptFunctionCall
{
    /// <summary>
    /// The name of the function to call.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The arguments of the function call in JSON format.
    /// </summary>
    public JsonDocument? Arguments { get; set; } = null!;
}
