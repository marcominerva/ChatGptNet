using ChatGptNet.Models;

namespace ChatGptNet.Extensions;

/// <summary>
/// Provides extension methods for working with <see cref="ChatGptFunction"/> instances.
/// </summary>
public static class ChatGptFunctionExtensions
{
    /// <summary>
    /// Converts a list of <see cref="ChatGptFunction"/> to the corresponding tool definitions.
    /// </summary>
    /// <param name="functions">The list of <see cref="ChatGptFunction"/></param>    
    /// <returns>The list of <see cref="ChatGptTool"/> objects that contains the specified <paramref name="functions"/>.</returns>
    public static IEnumerable<ChatGptTool> ToTools(this IEnumerable<ChatGptFunction> functions)
    {
        return functions.Select(function => new ChatGptTool
        {
            Type = ChatGptToolTypes.Function,
            Function = function
        });
    }
}
