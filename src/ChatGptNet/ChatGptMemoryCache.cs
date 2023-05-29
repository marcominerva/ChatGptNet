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

    public Task SetAsync(Guid conversationId, IEnumerable<ChatGptMessage> messages, TimeSpan expiration)
    {
        cache.Set(conversationId, messages, expiration);
        return Task.CompletedTask;
    }

    public Task<List<ChatGptMessage>> GetAsync(Guid conversationId)
    {
        var items = cache.Get<List<ChatGptMessage>>(conversationId);
        return Task.FromResult(items);
    }

    public Task RemoveAsync(Guid conversationId)
    {
        cache.Remove(conversationId);
        return Task.CompletedTask;
    }
}
