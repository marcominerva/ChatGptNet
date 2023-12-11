namespace ChatGptNet.Models;

/// <summary>
/// Represents a tool that the model may call.
/// </summary>
public class ChatGptTool
{
    /// <summary>
    /// The type of the tool.
    /// </summary>
    /// <seealso cref="ChatGptToolTypes"/>
    public string Type { get; set; } = ChatGptToolTypes.Function;

    /// <summary>
    /// The function associated with the tool, if any.
    /// </summary>
    public ChatGptFunction? Function { get; set; }
}
