namespace ChatGptNet.Models;

/// <summary>
/// Contains parameters about the function calls that are available for ChatGPT.
/// </summary>
public class ChatGptFunctionParameters
{
    /// <summary>
    /// Controls how the model responds to function calls. <em>none</em> means the model does not call a function, and responds to the end-user. <em>auto</em> means the model can pick between an end-user or calling a function. Specifying a particular function name forces the model to call that function.
    /// </summary>
    /// <remarks>
    /// <em>none</em> is the default when no functions are present. <em>auto</em> is the default if functions are present.
    /// </remarks>
    /// <seealso cref="Functions"/>
    /// <seealso cref="ChatGptFunction"/>
    public string? FunctionCall { get; set; }

    /// <summary>
    /// Gets or sets a list of functions the model may generate JSON inputs for.
    /// </summary>
    /// <seealso cref="ChatGptFunction"/>
    /// <seealso cref="FunctionCall"/>
    public IEnumerable<ChatGptFunction>? Functions { get; set; }
}
