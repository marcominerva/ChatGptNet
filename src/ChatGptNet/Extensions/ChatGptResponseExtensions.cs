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
    /// Returns an <see cref="IAsyncEnumerable{T}"/> that allows to enumerate all the response messages, each of them containing a partial delta.
    /// </summary>
    /// <param name="responses">The source <see cref="IAsyncEnumerable{ChatGptResponse}"/>.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> that allows to enumerate all the response messages, each of them containing a partial delta.</returns>
    /// <seealso cref="ChatGptResponse"/>
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "This method returns an IAsyncEnumerable")]
    public static async IAsyncEnumerable<string> AsMessages(this IAsyncEnumerable<ChatGptResponse> responses)
    {
        await foreach (var response in responses)
        {
            yield return response.GetMessage()!;
        }
    }
}
