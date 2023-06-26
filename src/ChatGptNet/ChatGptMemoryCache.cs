using ChatGptNet.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ChatGptNet;

internal class ChatGptMemoryCache : IChatGptCache
{
    private readonly IMemoryCache cache;

    public ChatGptMemoryCache(IMemoryCache cache)
    {
        this.cache = cache;
    }

    public Task SetAsync(Guid conversationId, IEnumerable<ChatGptMessage> messages, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        cache.Set(conversationId, messages, expiration);
        return Task.CompletedTask;
    }

    public Task<List<ChatGptMessage>?> GetAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var messages = cache.Get<List<ChatGptMessage>?>(conversationId);
        return Task.FromResult(messages);
    }

    public Task RemoveAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        cache.Remove(conversationId);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var exists = cache.TryGetValue(conversationId, out _);
        return Task.FromResult(exists);
    }
}
