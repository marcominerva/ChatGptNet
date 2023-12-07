using ChatGptNet.Models;

namespace ChatGptNet.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="ChatGptChoice"/> class.
/// </summary>
public static class ChatGptChoiceExtensions
{
    /// <summary>
    /// Gets a value indicating whether this choice contains a function call. 
    /// </summary>
    public static bool ContainsFunctionCalls(this ChatGptChoice choice)
        => choice.Message?.FunctionCall is not null;

    /// <summary>
    /// Gets the first function call of the message, if any.
    /// </summary>
    /// <returns>The first function call of the message, if any.</returns>
    public static ChatGptFunctionCall? GetFunctionCall(this ChatGptChoice choice)
        => choice.Message?.FunctionCall;
}