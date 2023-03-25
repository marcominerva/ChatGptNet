using Microsoft.Extensions.Caching.Memory;

namespace ChatGptNet;

internal class ChatGptClientFactory : IChatGptClientFactory
{
    private readonly IMemoryCache memoryCache;
    private readonly ChatGptOptions defaultOptions;

    public ChatGptClientFactory(IMemoryCache memoryCache, ChatGptOptions defaultOptions)
    {
        this.memoryCache = memoryCache;
        this.defaultOptions = defaultOptions;
    }

    public IChatGptClient CreateClient(Action<ChatGptOptions> setupAction)
    {
        ArgumentNullException.ThrowIfNull(setupAction);

        var options = defaultOptions with { };
        setupAction(options);

        var httpClient = new HttpClient();

        ChatGptServiceCollectionExtensions.ConfigureHttpClient(httpClient, options);

        return new ChatGptClient(httpClient, memoryCache, options);
    }
}
