using System.Text.Json;

namespace ChatGptNet.Models;

/// <summary>
/// Represents the description of a function available for ChatGPT.
/// </summary>
public class ChatGptFunction
{
    /// <summary>
    /// Gets or sets the name of the function to be called. Must be a-z, A-Z, 0-9, or contain underscores and dashes, with a maximum length of 64.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets The description of what the function does.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets a <see cref="JsonDocument"/> containing the parameters the function accepts.
    /// </summary>
    /// <remarks>
    /// See the <see href="https://platform.openai.com/docs/guides/gpt/function-calling">guide</see> for examples, and the <see href="https://json-schema.org/understanding-json-schema/">JSON Schema references</see> for documentation about the format.
    /// To describe a function that accepts no parameters, provide the value <code>{ "type": "object", "properties": {} }</code>
    /// or use the <see cref="EmptyParameters"/> property.
    /// </remarks>
    public JsonDocument Parameters { get; set; } = EmptyParameters;

    /// <summary>
    /// Gets a <see cref="JsonDocument"/> that represents an empty set of parameters.
    /// </summary>
    public static JsonDocument EmptyParameters { get; } = JsonDocument.Parse("""
        {
            "type": "object",
            "properties": { }
        }
        """);
}
