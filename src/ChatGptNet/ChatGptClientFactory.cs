using Microsoft.Extensions.DependencyInjection;

namespace ChatGptNet;

internal class ChatGptClientFactory(IServiceProvider serviceProvider, IChatGptCache chatGptCache, ChatGptOptionsBuilder defaultOptions) : IChatGptClientFactory
{
    public IChatGptClient CreateClient(Action<IServiceProvider, ChatGptOptionsBuilder>? setupAction)
    {
        var options = new ChatGptOptionsBuilder(defaultOptions);

        if (setupAction is not null)
        {
            setupAction(serviceProvider, options);
        }

        var httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient();
        return new ChatGptClient(httpClient, chatGptCache, options.Build());
    }

    public IChatGptClient CreateClient(Action<ChatGptOptionsBuilder>? setupAction)
    {
        if (setupAction is null)
        {
            return CreateClient();
        }

        return CreateClient((_, options) => setupAction(options));
    }

    public IChatGptClient CreateClient()
        => CreateClient((Action<IServiceProvider, ChatGptOptionsBuilder>?)null);
}
