using ChatGptNet.Exceptions;
using ChatGptNet.Models;

namespace ChatGptNet;

/// <summary>
/// Provides methods to interact with ChatGPT.
/// </summary>
public interface IChatGptClient
{
    /// <summary>
    /// Setups a new conversation with a system message, that is used to influence assistant behavior.
    /// </summary>
    /// <param name="message">The system message.</param>
    /// <returns>The new Conversation Id.</returns>
    /// <remarks>This method creates a new conversation with a system message and a random Conversation Id. Then, call <see cref="AskAsync(Guid, string, ChatGptParameters, CancellationToken)"/> or <see cref="AskStreamAsync(Guid, string, ChatGptParameters, CancellationToken)"/> with this Id to start the actual conversation.</remarks>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    /// <seealso cref="AskAsync(Guid, string, ChatGptParameters, CancellationToken)"/>
    Task<Guid> SetupAsync(string message)
        => SetupAsync(Guid.NewGuid(), message);

    /// <summary>
    /// Setups a conversation with a system message, that is used to influence assistant behavior.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation, used to automatically retrieve previous messages in the chat history.</param>
    /// <param name="message">The system message.</param>
    /// <remarks>This method creates a new conversation, with a system message and the given <paramref name="conversationId"/>. If a conversation with this Id already exists, it will be automatically cleared. Then, call <see cref="AskAsync(Guid, string, ChatGptParameters, CancellationToken)"/> or <see cref="AskStreamAsync(Guid, string, ChatGptParameters, CancellationToken)"/> with the same <paramref name="conversationId"/> to start the actual conversation.</remarks>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    /// <seealso cref="AskAsync(Guid, string, ChatGptParameters, CancellationToken)"/>
    Task<Guid> SetupAsync(Guid conversationId, string message);

    /// <summary>
    /// Requests a new chat interaction using the default completion model specified in the <see cref="ChatGptOptions.DefaultModel"/> property.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="chatGptParameters">A <seealso cref="ChatGptParameters"/> object that it's used to override the default completion parameters in the <see cref="ChatGptOptions.DefaultModel"/></param>
    /// <param name="cancellationToken">A <seealso cref="CancellationToken"/> that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>The chat completion response.</returns>
    /// <remarks>This method automatically starts a new conservation with a random Conversation Id, that will be returned in the <see cref="ChatGptResponse"/>. Subsequent calls to this method must provide the same <see cref="ChatGptResponse.ConversationId"/> value, so that previous messages will be automatically used to continue the conversation.</remarks>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    /// <exception cref="ChatGptException">An error occurred while calling the API and the <see cref="ChatGptOptions.ThrowExceptionOnError"/> is <see langword="true"/>.</exception>
    /// <seealso cref="ChatGptRequest"/>
    /// <seealso cref="ChatGptResponse"/>
    /// <seealso cref="ChatGptOptions"/>
    Task<ChatGptResponse> AskAsync(string message, ChatGptParameters? chatGptParameters = null, CancellationToken cancellationToken = default) =>
        AskAsync(Guid.NewGuid(), message, chatGptParameters, cancellationToken);

    /// <summary>
    /// Requests a chat interaction using the default completion model specified in the <see cref="ChatGptOptions.DefaultModel"/> property.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation, used to automatically retrieve previous messages in the chat history.</param>
    /// <param name="message">The message.</param>
    /// <param name="chatGptParameters">A <seealso cref="ChatGptParameters"/> object that it's used to override the default completion parameters in the <see cref="ChatGptOptions.DefaultModel"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>The chat completion response.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    /// <exception cref="ChatGptException">An error occurred while calling the API and the <seealso cref="ChatGptOptions.ThrowExceptionOnError"/> is <see langword="true"/>.</exception>
    /// <seealso cref="ChatGptRequest"/>
    /// <seealso cref="ChatGptResponse"/>
    /// <seealso cref="ChatGptOptions"/>
    Task<ChatGptResponse> AskAsync(Guid conversationId, string message, ChatGptParameters? chatGptParameters = null, CancellationToken cancellationToken = default)
        => AskAsync(conversationId, message, null, chatGptParameters, cancellationToken);

    /// <summary>
    /// Requests a chat interaction.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation, used to automatically retrieve previous messages in the chat history.</param>
    /// <param name="message">The message.</param>
    /// <param name="model">The chat completion model to use. If model is <see langword="null"/>, then the one specified in the <see cref="ChatGptOptions.DefaultModel"/> property will be used.</param>
    /// <param name="chatGptParameters">A <seealso cref="ChatGptParameters"/> object that it's used to override the default completion parameters in the <see cref="ChatGptOptions.DefaultModel"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>The chat completion response.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    /// <exception cref="ChatGptException">An error occurred while calling the API and the <see cref="ChatGptOptions.ThrowExceptionOnError"/> is <see langword="true"/>.</exception>
    /// <seealso cref="ChatGptRequest"/>
    /// <seealso cref="ChatGptResponse"/>
    Task<ChatGptResponse> AskAsync(Guid conversationId, string message, string? model, ChatGptParameters? chatGptParameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Requests a new chat interaction (using the default completion model specified in the <see cref="ChatGptOptions.DefaultModel"/> property) with streaming response, like in ChatGPT.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="chatGptParameters">A <seealso cref="ChatGptParameters"/> object that it's used to override the default completion parameters in the <see cref="ChatGptOptions.DefaultModel"/></param>
    /// <param name="cancellationToken">A <seealso cref="CancellationToken"/> that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>An <see cref="IAsyncEnumerable{ChatGptResponse}"/> that allows to enumerate all the streaming responses, each of them containing a partial message delta.</returns>
    /// <remarks>
    /// This method automatically starts a new conservation with a random Conversation Id, that will be returned in the <see cref="ChatGptResponse"/>. Subsequent calls to this method must provide the same <see cref="ChatGptResponse.ConversationId"/> value, so that previous messages will be automatically used to continue the conversation.
    /// When using steaming, partial message deltas will be sent. Tokens will be sent as data-only server-sent events as they become available.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    /// <exception cref="ChatGptException">An error occurred while calling the API and the <see cref="ChatGptOptions.ThrowExceptionOnError"/> is <see langword="true"/>.</exception>
    /// <seealso cref="ChatGptRequest"/>
    /// <seealso cref="ChatGptResponse"/>
    IAsyncEnumerable<ChatGptResponse> AskStreamAsync(string message, ChatGptParameters? chatGptParameters = null, CancellationToken cancellationToken = default) =>
        AskStreamAsync(Guid.NewGuid(), message, chatGptParameters, cancellationToken);

    /// <summary>
    /// Requests a chat interaction (using the default completion model specified in the <see cref="ChatGptOptions.DefaultModel"/> property) with streaming response, like in ChatGPT.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation, used to automatically retrieve previous messages in the chat history.</param>
    /// <param name="message">The message.</param>
    /// <param name="chatGptParameters">A <seealso cref="ChatGptParameters"/> object that it's used to override the default completion parameters in the <see cref="ChatGptOptions.DefaultModel"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>An <see cref="IAsyncEnumerable{ChatGptResponse}"/> that allows to enumerate all the streaming responses, each of them containing a partial message delta.</returns>
    /// <remarks> When using steaming, partial message deltas will be sent. Tokens will be sent as data-only server-sent events as they become available.</remarks>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    /// <exception cref="ChatGptException">An error occurred while calling the API and the <seealso cref="ChatGptOptions.ThrowExceptionOnError"/> is <see langword="true"/>.</exception>
    /// <seealso cref="ChatGptRequest"/>
    /// <seealso cref="ChatGptResponse"/>
    /// <seealso cref="ChatGptOptions"/>
    IAsyncEnumerable<ChatGptResponse> AskStreamAsync(Guid conversationId, string message, ChatGptParameters? chatGptParameters = null, CancellationToken cancellationToken = default)
        => AskStreamAsync(conversationId, message, null, chatGptParameters, cancellationToken);

    /// <summary>
    /// Requests a chat interaction with streaming response, like in ChatGPT.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation, used to automatically retrieve previous messages in the chat history.</param>
    /// <param name="message">The message.</param>
    /// <param name="model">The chat completion model to use. If model is <see langword="null"/>, then the one specified in the <see cref="ChatGptOptions.DefaultModel"/> property will be used.</param>
    /// <param name="chatGptParameters">A <seealso cref="ChatGptParameters"/> object that it's used to override the default completion parameters in the <see cref="ChatGptOptions.DefaultModel"/></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>An <see cref="IAsyncEnumerable{ChatGptResponse}"/> that allows to enumerate all the streaming responses, each of them containing a partial message delta.</returns>
    /// <remarks> When using steaming, partial message deltas will be sent. Tokens will be sent as data-only server-sent events as they become available.</remarks>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    /// <exception cref="ChatGptException">An error occurred while calling the API and the <see cref="ChatGptOptions.ThrowExceptionOnError"/> is <see langword="true"/>.</exception>
    /// <seealso cref="ChatGptRequest"/>
    /// <seealso cref="ChatGptResponse"/>
    IAsyncEnumerable<ChatGptResponse> AskStreamAsync(Guid conversationId, string message, string? model, ChatGptParameters? chatGptParameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a chat conversation, clearing all the history.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation.</param>
    /// <returns>The <see cref="Task"/> corresponding to the asynchronous operation.</returns>
    Task DeleteConversationAsync(Guid conversationId);
}
