namespace ChatGptNet.Models;

/// <summary>
/// Contains parameters about the function calls that are available for ChatGPT.
/// </summary>
public class ChatGptToolParameters
{
    /// <summary>
    /// Controls which (if any) function is called by the model.
    /// </summary>
    /// <remarks>
    /// <list type = "bullet" >
    ///   <item>
    ///     <term><see cref="ChatGptToolChoices.None"/></term>
    ///     <description>Model will not call a function and instead generates a message.</description>
    ///   </item>
    ///   <item>
    ///     <term><see cref="ChatGptToolChoices.Auto"/></term>
    ///     <description>The model can pick between generating a message or calling a function.</description>
    ///   </item>
    ///   <item>
    ///     <term><em>function_name</em></term>
    ///     <description>Specifying a particular function name forces the model to call that function.</description>
    ///   </item>
    /// </list>
    /// <see cref="ChatGptToolChoices.None"/> is the default when no functions are present. <see cref="ChatGptToolChoices.None"/> is the default if functions are present.
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

    /// <summary>
    /// Controls which (if any) function is called by the model.
    /// </summary>
    /// <remarks>
    /// <list type = "bullet" >
    ///   <item>
    ///     <term><see cref="ChatGptToolChoices.None"/></term>
    ///     <description>Model will not call a function and instead generates a message.</description>
    ///   </item>
    ///   <item>
    ///     <term><see cref="ChatGptToolChoices.Auto"/></term>
    ///     <description>The model can pick between generating a message or calling a function.</description>
    ///   </item>
    ///   <item>
    ///     <term><em>function_name</em></term>
    ///     <description>Specifying a particular function name forces the model to call that function.</description>
    ///   </item>
    /// </list>
    /// <see cref="ChatGptToolChoices.None"/> is the default when no functions are present. <see cref="ChatGptToolChoices.None"/> is the default if functions are present.
    /// </remarks>
    /// <seealso cref="ChatGptFunction"/>
    public string? ToolChoice { get; set; }

    /// <summary>
    /// Gets or sets a list of tools the model may call.
    /// </summary>
    /// <seealso cref="ChatGptTool"/>
    /// <seealso cref="ChatGptFunction"/>
    /// <seealso cref="ToolChoice"/>
    public IEnumerable<ChatGptTool>? Tools { get; set; }
}
