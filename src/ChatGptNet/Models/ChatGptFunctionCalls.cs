namespace ChatGptNet.Models;

/// <summary>
/// Contains constants for ChatGPT function call types.
/// </summary>
public static class ChatGptFunctionCalls
{
    /// <summary>
    /// The model does not call a function, and responds to the end-user.
    /// </summary>
    /// <remarks>
    /// This is the default when no functions are present.
    /// </remarks>
    public const string None = "none";

    /// <summary>
    /// The model can pick between an end-user or calling a function.
    /// </summary>
    /// <remarks>
    /// This is the default if functions are present
    /// </remarks>
    public const string Auto = "auto";
}