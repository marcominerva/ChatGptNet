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
        => choice.Message?.FunctionCall is not null || choice.Delta?.FunctionCall is not null || (choice.Message?.ToolCalls?.Any(call => call.Type == ChatGptToolTypes.Function) ?? false);

    /// <summary>
    /// Gets the first function call of the message, if any.
    /// </summary>
    /// <returns>The first function call of the message, if any.</returns>
    public static ChatGptFunctionCall? GetFunctionCall(this ChatGptChoice choice)
        => choice.Message?.FunctionCall ?? choice.Delta?.FunctionCall ?? choice.Message?.ToolCalls?.FirstOrDefault(call => call.Type == ChatGptToolTypes.Function)?.Function;

    /// <summary>
    /// Gets a value indicating whether this choice contains at least one tool call. 
    /// </summary>
    public static bool ContainsToolCalls(this ChatGptChoice choice)
        => choice.Message?.ToolCalls?.Any() ?? false;
}