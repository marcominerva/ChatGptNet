using ChatGptNet.Exceptions;
using ChatGptNet.Models;
using ChatGptNet.Models.Embeddings;

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
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The unique identifier of the new conversation.</returns>
    /// <remarks>This method creates a new conversation with a system message and a random Conversation Id. Then, call <see cref="AskAsync(Guid, string, ChatGptParameters, string, bool, CancellationToken)"/> with this Id to start the actual conversation.</remarks>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    /// <seealso cref="AskAsync(Guid, string, ChatGptParameters, string, bool, CancellationToken)"/>
    Task<Guid> SetupAsync(string message, CancellationToken cancellationToken = default)
        => SetupAsync(Guid.NewGuid(), message, cancellationToken);

    /// <summary>
    /// Setups a conversation with a system message, that is used to influence assistant behavior.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation, used to automatically retrieve previous messages in the chat history.</param>
    /// <param name="message">The system message.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <remarks>This method creates a new conversation, with a system message and the given <paramref name="conversationId"/>. If a conversation with this Id already exists, it will be automatically cleared. Then, call <see cref="AskAsync(Guid, string, ChatGptParameters, string, bool, CancellationToken)"/> to start the actual conversation.</remarks>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    /// <seealso cref="AskAsync(Guid, string, ChatGptToolParameters, ChatGptParameters, string, bool, CancellationToken)"/>
    Task<Guid> SetupAsync(Guid conversationId, string message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Requests a new chat interaction.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="parameters">A <see cref="ChatGptParameters"/> object used to override the default completion parameters in the <see cref="ChatGptOptions.DefaultParameters"/> property.</param>
    /// <param name="model">The chat completion model to use. If <paramref name="model"/> is <see langword="null"/>, then the one specified in the <see cref="ChatGptOptions.DefaultModel"/> property will be used.</param>
    /// <param name="addToConversationHistory">Set to <see langword="true"/> to add the current chat interaction to the conversation history.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The chat completion response.</returns>
    /// <remarks>This method automatically starts a new conservation with a random Conversation Id, that will be returned in the <see cref="ChatGptResponse"/>. Subsequent calls to this method must provide the same <see cref="ChatGptResponse.ConversationId"/> value, so that previous messages will be automatically used to continue the conversation.</remarks>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    /// <exception cref="ChatGptException">An error occurred while calling the API and the <see cref="ChatGptOptions.ThrowExceptionOnError"/> is <see langword="true"/>.</exception>
    /// <seealso cref="ChatGptRequest"/>
    /// <seealso cref="ChatGptResponse"/>
    /// <seealso cref="ChatGptOptions"/>
    /// <seealso cref="ChatGptParameters"/>
    Task<ChatGptResponse> AskAsync(string message, ChatGptParameters? parameters = null, string? model = null, bool addToConversationHistory = true, CancellationToken cancellationToken = default) =>
        AskAsync(Guid.NewGuid(), message, null, parameters, model, addToConversationHistory, cancellationToken);

    /// <summary>
    /// Requests a new chat interaction.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="toolParameters">A <see cref="ChatGptToolParameters"/> object that contains the list of available functions for calling.</param>
    /// <param name="parameters">A <see cref="ChatGptParameters"/> object used to override the default completion parameters in the <see cref="ChatGptOptions.DefaultParameters"/> property.</param>
    /// <param name="model">The chat completion model to use. If <paramref name="model"/> is <see langword="null"/>, then the one specified in the <see cref="ChatGptOptions.DefaultModel"/> property will be used.</param>
    /// <param name="addToConversationHistory">Set to <see langword="true"/> to add the current chat interaction to the conversation history.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The chat completion response.</returns>
    /// <remarks>
    /// <para>This method automatically starts a new conservation with a random Conversation Id, that will be returned in the <see cref="ChatGptResponse"/>. Subsequent calls to this method must provide the same <see cref="ChatGptResponse.ConversationId"/> value, so that previous messages will be automatically used to continue the conversation.</para>
    /// <para>The Chat Completions API does not call the function; instead, the model generates JSON that you can use to call the function in your code.</para>
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    /// <exception cref="ChatGptException">An error occurred while calling the API and the <see cref="ChatGptOptions.ThrowExceptionOnError"/> is <see langword="true"/>.</exception>
    /// <seealso cref="ChatGptRequest"/>
    /// <seealso cref="ChatGptResponse"/>
    /// <seealso cref="ChatGptOptions"/>
    /// <seealso cref="ChatGptToolParameters"/>
    /// <seealso cref="ChatGptParameters"/>
    Task<ChatGptResponse> AskAsync(string message, ChatGptToolParameters? toolParameters, ChatGptParameters? parameters = null, string? model = null, bool addToConversationHistory = true, CancellationToken cancellationToken = default) =>
        AskAsync(Guid.NewGuid(), message, toolParameters, parameters, model, addToConversationHistory, cancellationToken);

    /// <summary>
    /// Requests a chat interaction.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation, used to automatically retrieve previous messages in the chat history.</param>
    /// <param name="message">The message.</param>
    /// <param name="parameters">A <seealso cref="ChatGptParameters"/> object used to override the default completion parameters in the <see cref="ChatGptOptions.DefaultParameters"/> property.</param>
    /// <param name="model">The chat completion model to use. If <paramref name="model"/> is <see langword="null"/>, then the one specified in the <see cref="ChatGptOptions.DefaultModel"/> property will be used.</param>
    /// <param name="addToConversationHistory">Set to <see langword="true"/> to add the current chat interaction to the conversation history.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The chat completion response.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    /// <exception cref="ChatGptException">An error occurred while calling the API and the <see cref="ChatGptOptions.ThrowExceptionOnError"/> is <see langword="true"/>.</exception>
    /// <seealso cref="ChatGptRequest"/>
    /// <seealso cref="ChatGptResponse"/>
    /// <seealso cref="ChatGptParameters"/>
    Task<ChatGptResponse> AskAsync(Guid conversationId, string message, ChatGptParameters? parameters = null, string? model = null, bool addToConversationHistory = true, CancellationToken cancellationToken = default)
        => AskAsync(conversationId, message, null, parameters, model, addToConversationHistory, cancellationToken);

    /// <summary>
    /// Requests a chat interaction.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation, used to automatically retrieve previous messages in the chat history.</param>
    /// <param name="message">The message.</param>
    /// <param name="toolParameters">A <see cref="ChatGptToolParameters"/> object that contains the list of available functions for calling.</param>
    /// <param name="parameters">A <seealso cref="ChatGptParameters"/> object used to override the default completion parameters in the <see cref="ChatGptOptions.DefaultParameters"/> property.</param>
    /// <param name="model">The chat completion model to use. If <paramref name="model"/> is <see langword="null"/>, then the one specified in the <see cref="ChatGptOptions.DefaultModel"/> property will be used.</param>
    /// <param name="addToConversationHistory">Set to <see langword="true"/> to add the current chat interaction to the conversation history.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The chat completion response.</returns>
    /// <remarks>
    /// The Chat Completions API does not call the function; instead, the model generates JSON that you can use to call the function in your code.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    /// <exception cref="ChatGptException">An error occurred while calling the API and the <see cref="ChatGptOptions.ThrowExceptionOnError"/> is <see langword="true"/>.</exception>
    /// <seealso cref="ChatGptRequest"/>
    /// <seealso cref="ChatGptResponse"/>
    /// <seealso cref="ChatGptToolParameters"/>
    /// <seealso cref="ChatGptParameters"/>
    Task<ChatGptResponse> AskAsync(Guid conversationId, string message, ChatGptToolParameters? toolParameters, ChatGptParameters? parameters = null, string? model = null, bool addToConversationHistory = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Requests a new chat interaction with streaming response, like in ChatGPT.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="parameters">A <see cref="ChatGptParameters"/> object used to override the default completion parameters in the <see cref="ChatGptOptions.DefaultParameters"/> property.</param>
    /// <param name="model">The chat completion model to use. If <paramref name="model"/> is <see langword="null"/>, then the one specified in the <see cref="ChatGptOptions.DefaultModel"/> property will be used.</param>
    /// <param name="addToConversationHistory">Set to <see langword="true"/> to add the current chat interaction to the conversation history.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="IAsyncEnumerable{ChatGptResponse}"/> that allows to enumerate all the streaming responses, each of them containing a partial message delta.</returns>
    /// <remarks>
    /// This method automatically starts a new conservation with a random Conversation Id, that will be returned in the <see cref="ChatGptResponse"/>. Subsequent calls to this method must provide the same <see cref="ChatGptResponse.ConversationId"/> value, so that previous messages will be automatically used to continue the conversation.
    /// When using steaming, partial message deltas will be sent. Tokens will be sent as data-only server-sent events as they become available.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    /// <exception cref="ChatGptException">An error occurred while calling the API and the <see cref="ChatGptOptions.ThrowExceptionOnError"/> is <see langword="true"/>.</exception>
    /// <seealso cref="ChatGptRequest"/>
    /// <seealso cref="ChatGptResponse"/>
    /// <seealso cref="ChatGptParameters"/>
    IAsyncEnumerable<ChatGptResponse> AskStreamAsync(string message, ChatGptParameters? parameters = null, string? model = null, bool addToConversationHistory = true, CancellationToken cancellationToken = default) =>
        AskStreamAsync(Guid.NewGuid(), message, parameters, model, addToConversationHistory, cancellationToken);

    /// <summary>
    /// Requests a chat interaction with streaming response, like in ChatGPT.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation, used to automatically retrieve previous messages in the chat history.</param>
    /// <param name="message">The message.</param>
    /// <param name="parameters">A <see cref="ChatGptParameters"/> object used to override the default completion parameters in the <see cref="ChatGptOptions.DefaultParameters"/> property.</param>
    /// <param name="model">The chat completion model to use. If <paramref name="model"/> is <see langword="null"/>, then the one specified in the <see cref="ChatGptOptions.DefaultModel"/> property will be used.</param>
    /// <param name="addToConversationHistory">Set to <see langword="true"/> to add the current chat interaction to the conversation history.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="IAsyncEnumerable{ChatGptResponse}"/> that allows to enumerate all the streaming responses, each of them containing a partial message delta.</returns>
    /// <remarks> When using steaming, partial message deltas will be sent. Tokens will be sent as data-only server-sent events as they become available.</remarks>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    /// <exception cref="ChatGptException">An error occurred while calling the API and the <see cref="ChatGptOptions.ThrowExceptionOnError"/> is <see langword="true"/>.</exception>
    /// <seealso cref="ChatGptRequest"/>
    /// <seealso cref="ChatGptResponse"/>
    /// <seealso cref="ChatGptParameters"/>
    IAsyncEnumerable<ChatGptResponse> AskStreamAsync(Guid conversationId, string message, ChatGptParameters? parameters = null, string? model = null, bool addToConversationHistory = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Explicitly adds a new interaction (a question and the corresponding answer) to the conversation history.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation.</param>
    /// <param name="question">The question.</param>
    /// <param name="answer">The answer.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The <see cref="Task"/> corresponding to the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="question"/> or <paramref name="answer"/> are <see langword="null"/>.</exception>
    Task AddInteractionAsync(Guid conversationId, string question, string answer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a chat conversation from the cache.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The message list of the conversation, or <see cref="Enumerable.Empty{ChatGptMessage}"/> if the Conversation Id does not exist.</returns>
    /// <seealso cref="ChatGptMessage"/>
    Task<IEnumerable<ChatGptMessage>> GetConversationAsync(Guid conversationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads messages into a new conversation.
    /// </summary>
    /// <param name="messages">Messages to load into a new conversation.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The unique identifier of the new conversation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="messages"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// <para>This method creates a new conversation with a random Conversation Id. Then, call <see cref="AskAsync(Guid, string, ChatGptParameters, string, bool, CancellationToken)"/> or <see cref="AskStreamAsync(Guid, string, ChatGptParameters?, string?, bool, CancellationToken)"/> with this Id to start the actual conversation.</para>
    /// <para>The total number of messages never exceeds the message limit defined in <see cref="ChatGptOptions.MessageLimit"/>. If <paramref name="messages"/> contains more, only the latest ones are loaded.</para>
    /// </remarks>
    /// <seealso cref="ChatGptOptions.MessageLimit"/>
    /// <seealso cref="AskAsync(Guid, string, ChatGptParameters?, string?, bool, CancellationToken)"/>
    /// <seealso cref="AskStreamAsync(Guid, string, ChatGptParameters?, string?, bool, CancellationToken)"/>
    Task<Guid> LoadConversationAsync(IEnumerable<ChatGptMessage> messages, CancellationToken cancellationToken = default)
        => LoadConversationAsync(Guid.NewGuid(), messages, true, cancellationToken);

    /// <summary>
    /// Loads messages into conversation history.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation.</param>
    /// <param name="messages">The messages to load into conversation history.</param>
    /// <param name="replaceHistory"><see langword="true"/> to replace all the existing messages; <see langword="false"/> to mantain them.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The unique identifier of the conversation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="messages"/> is <see langword="null"/>.</exception>
    /// <remarks>The total number of messages never exceeds the message limit defined in <see cref="ChatGptOptions.MessageLimit"/>. If <paramref name="messages"/> contains more, only the latest ones are loaded.</remarks>
    /// <seealso cref="ChatGptOptions.MessageLimit"/>
    Task<Guid> LoadConversationAsync(Guid conversationId, IEnumerable<ChatGptMessage> messages, bool replaceHistory = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a chat conversation exists.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns><see langword="true"/> if the conversation exists; otherwise, <see langword="false"/>.</returns>
    public Task<bool> ConversationExistsAsync(Guid conversationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a chat conversation, clearing all the history.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation.</param>
    /// <param name="preserveSetup"><see langword="true"/> to maintain the system message that has been specified with the <see cref="SetupAsync(Guid, string, CancellationToken)"/> method.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The <see cref="Task"/> corresponding to the asynchronous operation.</returns>
    /// <seealso cref="SetupAsync(Guid, string, CancellationToken)"/>
    Task DeleteConversationAsync(Guid conversationId, bool preserveSetup = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a function response to the conversation history.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation.</param>
    /// <param name="functionName">The name of the function.</param>
    /// <param name="content">The content of the function response.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The <see cref="Task"/> corresponding to the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="functionName"/> or <paramref name="content"/> are <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">The conversation history is empty.</exception>
    /// <seealso  cref="AskAsync(Guid, string, ChatGptParameters?, string?, bool, CancellationToken)"/>
    /// <seealso cref="AskStreamAsync(Guid, string, ChatGptParameters?, string?, bool, CancellationToken)"/>
    /// <seealso cref="ChatGptToolCall"/>
    Task AddFunctionResponseAsync(Guid conversationId, string functionName, string content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates embeddings for a message.
    /// </summary>
    /// <param name="message">The message to use for generating embeddings.</param>
    /// <param name="model">The name of the embedding model. If <paramref name="model"/> is <see langword="null"/>, then the one specified in the <see cref="ChatGptOptions.DefaultEmbeddingModel"/> property will be used.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The embeddings for the provided message.</returns>
    /// <exception cref="EmbeddingException">An error occurred while calling the API and the <see cref="ChatGptOptions.ThrowExceptionOnError"/> is <see langword="true"/>.</exception>
    Task<EmbeddingResponse> GenerateEmbeddingAsync(string message, string? model = null, CancellationToken cancellationToken = default)
        => GenerateEmbeddingAsync(new[] { message }, model, cancellationToken);

    /// <summary>
    /// Generates embeddings for a list of messages.
    /// </summary>
    /// <param name="messages">The messages to use for generating embeddings.</param>
    /// <param name="model">The name of the embedding model. If <paramref name="model"/> is <see langword="null"/>, then the one specified in the <see cref="ChatGptOptions.DefaultEmbeddingModel"/> property will be used.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The embeddings for the provided messages.</returns>
    /// <exception cref="EmbeddingException">An error occurred while calling the API and the <see cref="ChatGptOptions.ThrowExceptionOnError"/> is <see langword="true"/>.</exception>
    Task<EmbeddingResponse> GenerateEmbeddingAsync(IEnumerable<string> messages, string? model = null, CancellationToken cancellationToken = default);
}
