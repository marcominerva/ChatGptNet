using System.Diagnostics.CodeAnalysis;
using ChatGptNet.Models;

namespace ChatGptNet.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="ChatGptResponse"/> class.
/// </summary>
/// <seealso cref="ChatGptResponse"/>
public static class ChatGptResponseExtensions
{
    /// <summary>
    /// Gets the content of the first choice, if available.
    /// </summary>
    /// <returns>The content of the first choice, if available.</returns>
    /// <remarks>When using streaming responses, this method returns a partial message delta.</remarks>
    /// <seealso cref="ChatGptRequest.Stream"/>
    public static string? GetContent(this ChatGptResponse response) => response.Choices.FirstOrDefault()?.Delta?.Content ?? response.Choices.FirstOrDefault()?.Message?.Content?.Trim();

    /// <summary>
    /// Gets a value indicating whether the first choice, if available, contains a tool call. 
    /// </summary>
    /// <seealso cref="GetFunctionCall"/>
    /// <seealso cref="ChatGptToolCall"/>
    public static bool ContainsToolCalls(this ChatGptResponse response) => response.Choices.FirstOrDefault()?.ContainsToolCalls() ?? false;

    /// <summary>
    /// Gets the tool calls for the message of the first choice, if available.
    /// </summary>
    public static IEnumerable<ChatGptToolCall>? GetToolCalls(this ChatGptResponse response) => response.Choices.FirstOrDefault()?.Message?.ToolCalls;

    /// <summary>
    /// Gets a value indicating whether the first choice, if available, contains a function call. 
    /// </summary>
    /// <seealso cref="GetFunctionCall"/>
    /// <seealso cref="ChatGptFunctionCall"/>
    public static bool ContainsFunctionCalls(this ChatGptResponse response) => response.Choices.FirstOrDefault()?.ContainsFunctionCalls() ?? false;

    /// <summary>
    /// Gets or sets the function call for the message of the first choice, if available.
    /// </summary>
    public static ChatGptFunctionCall? GetFunctionCall(this ChatGptResponse response) => response.Choices.FirstOrDefault()?.GetFunctionCall();

    /// <summary>
    /// Returns an <see cref="IAsyncEnumerable{T}"/> that allows to enumerate all the partial message deltas.
    /// </summary>
    /// <param name="responses">The source <see cref="IAsyncEnumerable{ChatGptResponse}"/>.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> that allows to enumerate all the partial message deltas.</returns>
    /// <seealso cref="ChatGptResponse"/>
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "This method returns an IAsyncEnumerable")]
    public static async IAsyncEnumerable<string?> AsDeltas(this IAsyncEnumerable<ChatGptResponse> responses)
    {
        await foreach (var response in responses)
        {
            yield return response.GetContent();
        }
    }
}
