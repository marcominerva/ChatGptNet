using ChatGptNet.Exceptions;
using ChatGptNet.Models;

namespace ChatGptNet;

/// <summary>
/// Provides methods to interact with ChatGPT.
/// </summary>
public interface IChatGptClient
{
    /// <summary>
    /// Setup a new conversation with a system message.
    /// </summary>
    /// <param name="message">The system message</param>
    /// <returns>The new Conversation Id</returns>
    /// <remarks>This method creates a new conversation, using the default <seealso cref="ChatGptModels.Gpt35Turbo"/> model, with a system message and a random Conversation Id, next you need to call <seealso cref="AskAsync(Guid, string, CancellationToken)"/> to continue the conversation.</remarks>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    Task<Guid> SetupAsync(string message);

    /// <summary>
    /// Setup a conversation with a system message.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation, used to automatically retrieve previous messages in the chat history.</param>
    /// <param name="message">The system message</param>
    /// <remarks>This method creates a new conversation, using the default <seealso cref="ChatGptModels.Gpt35Turbo"/> model, with a system message, next you need to call <seealso cref="AskAsync(Guid, string, CancellationToken)"/> to continue the conversation. Subsequent calls to this method with the same Conversation Id will clear the conversation hisory</remarks>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    Task SetupAsync(Guid conversationId, string message) => SetupAsync(conversationId, message, ChatGptModels.Gpt35Turbo);

    /// <summary>
    /// Setup a conversation with a system message.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation, used to automatically retrieve previous messages in the chat history.</param>
    /// <param name="message">The system message</param>
    /// <param name="model">The chat completion model to use (default: <seealso cref="ChatGptModels.Gpt35Turbo"/>).</param>
    /// <remarks>This method creates a new conversation, with a system message, next you need to call <seealso cref="AskAsync(Guid, string, CancellationToken)"/> to continue the conversation. Subsequent calls to this method with the same Conversation Id will clear the conversation hisory</remarks>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    Task SetupAsync(Guid conversationId, string message, string model);

    /// <summary>
    /// Requests a new chat interaction.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="cancellationToken">A <seealso cref="CancellationToken"/> that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>The chat completion response.</returns>
    /// <remarks>This method automatically starts a new conservation with a random Conversation Id, that will be returned in the <see cref="ChatGptResponse"/>. Subsequent calls to this method must provide the same Conversation Id, so that previous messages will be automatically used to continue the conversation.</remarks>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    /// <exception cref="ChatGptException">An error occurred while calling the API and the <seealso cref="ChatGptOptions.ThrowExceptionOnError"/> is <see langword="true"/>.</exception>
    /// <seealso cref="ChatGptRequest"/>
    /// <seealso cref="ChatGptResponse"/>
    Task<ChatGptResponse> AskAsync(string message, CancellationToken cancellationToken = default) =>
        AskAsync(Guid.NewGuid(), message, cancellationToken);

    /// <summary>
    /// Requests a chat interaction.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation, used to automatically retrieve previous messages in the chat history.</param>
    /// <param name="message">The message.</param>
    /// <param name="cancellationToken">A <seealso cref="CancellationToken"/> that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>The chat completion response.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    /// <exception cref="ChatGptException">An error occurred while calling the API and the <seealso cref="ChatGptOptions.ThrowExceptionOnError"/> is <see langword="true"/>.</exception>
    /// <seealso cref="ChatGptRequest"/>
    /// <seealso cref="ChatGptResponse"/>
    /// <seealso cref="ChatGptOptions"/>
    Task<ChatGptResponse> AskAsync(Guid conversationId, string message, CancellationToken cancellationToken = default)
        => AskAsync(conversationId, message, ChatGptModels.Gpt35Turbo, cancellationToken);

    /// <summary>
    /// Requests a chat interaction.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation, used to automatically retrieve previous messages in the chat history.</param>
    /// <param name="message">The message.</param>
    /// <param name="model">The chat completion model to use (default: <seealso cref="ChatGptModels.Gpt35Turbo"/>).</param>
    /// <param name="cancellationToken">A <seealso cref="CancellationToken"/> that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>The chat completion response.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    /// <exception cref="ChatGptException">An error occurred while calling the API and the <seealso cref="ChatGptOptions.ThrowExceptionOnError"/> is <see langword="true"/>.</exception>
    /// <seealso cref="ChatGptRequest"/>
    /// <seealso cref="ChatGptResponse"/>
    Task<ChatGptResponse> AskAsync(Guid conversationId, string message, string model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a chat conversation, clearing all the history.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation.</param>
    /// <returns>The <see cref="Task"/> corresponding to the asynchronous operation.</returns>
    Task DeleteConversationAsync(Guid conversationId);
}
