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
    /// The arguments of the function call in JSON string format.
    /// </summary>
    public string? Arguments { get; set; }

    private JsonDocument argumentsAsJsonDocument = null!;
    /// <summary>
    /// The arguments of the function call as a <see cref="JsonDocument"/>.
    /// </summary>
    /// <seealso cref="JsonDocument"/>
    public JsonDocument GetArgumentsAsJson()
        => argumentsAsJsonDocument ??= JsonDocument.Parse(Arguments ?? "{}");
}
