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
}
