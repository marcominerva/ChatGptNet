using ChatGptNet.Models;

namespace ChatGptNet;

/// <summary>
/// Represents a local in-memory cache.
/// </summary>
public interface IChatGptCache
{
    /// <summary>
    /// Saves the list of messages for the given <paramref name="conversationId"/>, using the specified <paramref name="expiration"/>.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation.</param>
    /// <param name="messages">The list of messages.</param>
    /// <param name="expiration">The amount of time in which messages must be stored in cache.</param>
    /// <returns>The <see cref="Task"/> corresponding to the asynchronous operation.</returns>
    /// <seealso cref="ChatGptMessage"/>
    Task SetAsync(Guid conversationId, IEnumerable<ChatGptMessage> messages, TimeSpan expiration);

    /// <summary>
    /// Gets the list of messages for the given <paramref name="conversationId"/>.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation.</param>
    /// <returns>The message list of the conversation, or <see langword="null"/> if the Conversation Id does not exist.</returns>
    /// <seealso cref="ChatGptMessage"/>
    Task<List<ChatGptMessage>?> GetAsync(Guid conversationId);

    /// <summary>
    /// Removes from the cache all the message for the given <paramref name="conversationId"/>.
    /// </summary>
    /// <param name="conversationId">The unique identifier of the conversation.</param>
    /// <returns>The <see cref="Task"/> corresponding to the asynchronous operation.</returns>
    Task RemoveAsync(Guid conversationId);
}