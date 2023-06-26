using ChatGptNet.Models;

namespace ChatGptNet;

/// <summary>
/// Represents the interface used to define the caching behavior for ChatGPT messages.
/// </summary>
public interface IChatGptCache
{
    /// <summary>
    /// Saves the list of messages for the given <paramref name="conversationId"/>, using the specified <paramref name="expiration"/>.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation.</param>
    /// <param name="messages">The list of messages.</param>
    /// <param name="expiration">The amount of time in which messages must be stored in cache.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The <see cref="Task"/> corresponding to the asynchronous operation.</returns>
    /// <seealso cref="ChatGptMessage"/>
    Task SetAsync(Guid conversationId, IEnumerable<ChatGptMessage> messages, TimeSpan expiration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the list of messages for the given <paramref name="conversationId"/>.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The message list of the conversation, or <see langword="null"/> if the Conversation Id does not exist.</returns>
    /// <seealso cref="ChatGptMessage"/>
    Task<List<ChatGptMessage>?> GetAsync(Guid conversationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes from the cache all the message for the given <paramref name="conversationId"/>.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The <see cref="Task"/> corresponding to the asynchronous operation.</returns>
    Task RemoveAsync(Guid conversationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a value that indicates whether the given conversation exists in the cache.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The <see cref="Task"/> corresponding to the asynchronous operation.</returns>
    Task<bool> ExistsAsync(Guid conversationId, CancellationToken cancellationToken = default);
}