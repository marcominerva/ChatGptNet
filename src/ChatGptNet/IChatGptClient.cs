using ChatGptNet.Exceptions;
using ChatGptNet.Models;

namespace ChatGptNet;

/// <summary>
/// Provides methods to interact with ChatGPT.
/// </summary>
public interface IChatGptClient
{
    /// <summary>
    /// Request a chat interaction.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="model">The chat completion model to use (default: <seealso cref="ChatGptModels.Gpt35Turbo"/>).</param>
    /// <param name="cancellationToken">A <seealso cref="CancellationToken"/> that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>The chat completion response.</returns>
    /// <exception cref="ChatGptApiException">An error occurred while calling the API.</exception>
    /// <seealso cref="ChatGptRequest"/>
    /// <seealso cref="ChatGptResponse"/>
    Task<ChatGptResponse?> AskAsync(string message, string model = ChatGptModels.Gpt35Turbo, CancellationToken cancellationToken = default);
}
