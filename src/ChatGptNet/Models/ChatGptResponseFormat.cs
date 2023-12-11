namespace ChatGptNet.Models;

/// <summary>
/// An object specifying the format that the model must output. Used to enable JSON mode.
/// </summary>
/// <seealso cref="ChatGptResponseFormatTypes"/>
public class ChatGptResponseFormat
{
    /// <summary>
    /// The format type.
    /// </summary>
    /// <seealso cref="ChatGptResponseFormatTypes"/>
    public string Type { get; set; } = ChatGptResponseFormatTypes.Text;

    /// <summary>
    /// Gets a new <see cref="ChatGptResponseFormat"/> instance with the default text format type.
    /// </summary>
    public static ChatGptResponseFormat Text { get; } = new();

    /// <summary>
    /// Gets a new <see cref="ChatGptResponseFormat"/> instance with the JSON format type.
    /// </summary>
    public static ChatGptResponseFormat Json { get; } = new()
    {
        Type = ChatGptResponseFormatTypes.Json
    };
}
