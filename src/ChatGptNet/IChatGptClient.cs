using ChatGptNet.Exceptions;
using ChatGptNet.Models;

namespace ChatGptNet;

/// <summary>
/// Provides methods to interact with ChatGPT.
/// </summary>
public interface IChatGptClient
{
    /// <summary>
    /// Request a new chat interaction.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="cancellationToken">A <seealso cref="CancellationToken"/> that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>The chat completion response.</returns>
    /// <remarks>This method automatically starts a new conservation by generating a random new Conversation Id, that will be returned in the <see cref="ChatGptResponse"/>. Subsequent calls to this method must provide the same Conversation Id so that the previous messages will be automatically used to continue the conversation.</remarks>
    /// <exception cref="ChatGptException">An error occurred while calling the API.</exception>
    /// <seealso cref="ChatGptRequest"/>
    /// <seealso cref="ChatGptResponse"/>
    Task<ChatGptResponse> AskAsync(string message, CancellationToken cancellationToken = default) =>
        AskAsync(Guid.NewGuid(), message, cancellationToken);

    /// <summary>
    /// Request a chat interaction.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation, used to automatically retrieve previous messages in the chat history.</param>
    /// <param name="message">The message.</param>
    /// <param name="cancellationToken">A <seealso cref="CancellationToken"/> that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>The chat completion response.</returns>
    /// <exception cref="ChatGptException">An error occurred while calling the API.</exception>
    /// <seealso cref="ChatGptRequest"/>
    /// <seealso cref="ChatGptResponse"/>
    Task<ChatGptResponse> AskAsync(Guid conversationId, string message, CancellationToken cancellationToken = default)
        => AskAsync(conversationId, message, ChatGptModels.Gpt35Turbo, cancellationToken);

    /// <summary>
    /// Request a chat interaction.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation, used to automatically retrieve previous messages in the chat history.</param>
    /// <param name="message">The message.</param>
    /// <param name="model">The chat completion model to use (default: <seealso cref="ChatGptModels.Gpt35Turbo"/>).</param>
    /// <param name="cancellationToken">A <seealso cref="CancellationToken"/> that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>The chat completion response.</returns>
    /// <exception cref="ChatGptException">An error occurred while calling the API.</exception>
    /// <seealso cref="ChatGptRequest"/>
    /// <seealso cref="ChatGptResponse"/>
    Task<ChatGptResponse> AskAsync(Guid conversationId, string message, string model, CancellationToken cancellationToken = default);
}
