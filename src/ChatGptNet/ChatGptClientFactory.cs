using Microsoft.Extensions.Caching.Memory;

namespace ChatGptNet;

internal class ChatGptClientFactory : IChatGptClientFactory
{
    private readonly IServiceProvider services;
    private readonly IMemoryCache memoryCache;
    private readonly ChatGptOptions defaultOptions;

    public ChatGptClientFactory(IServiceProvider services, IMemoryCache memoryCache, ChatGptOptions defaultOptions)
    {
        this.services = services;
        this.memoryCache = memoryCache;
        this.defaultOptions = defaultOptions;
    }

    public IChatGptClient CreateClient(Action<IServiceProvider, ChatGptOptions>? setupAction)
    {
        var options = defaultOptions with { };

        if (setupAction is not null)
            setupAction(services, options);

        return new ChatGptClient(new HttpClient(), memoryCache, options);
    }
    public IChatGptClient CreateClient(Action<ChatGptOptions>? setupAction)
    {
        if (setupAction is null)
            return CreateClient();

        return CreateClient((s, o) => setupAction(o));
    }
    public IChatGptClient CreateClient()
    {
        return CreateClient((Action<IServiceProvider, ChatGptOptions>?)null);
    }
}
