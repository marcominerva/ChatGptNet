using Microsoft.Extensions.DependencyInjection;

namespace ChatGptNet;

/// <summary>
/// Provides extension methods for adding ChatGPT Client Factory support in NET applications.
/// </summary>
public static class ChatGptFactoryServiceCollectionExtensions
{
    /// <summary>
    /// Registers a <see cref="ChatGptClientFactory"/> instance.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="builder">The <see cref="Action{ChatGptOptions}"/> to configure the provided <see cref="ChatGptOptions"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddChatGptClientFactory(this IServiceCollection services, Action<ChatGptOptionsBuilder>? builder = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = new ChatGptOptionsBuilder();
        builder?.Invoke(options);

        return AddChatGptClientFactoryCore(services, options);
    }

    private static IServiceCollection AddChatGptClientFactoryCore(this IServiceCollection services, ChatGptOptionsBuilder deafultOptions)
    {
        services.AddMemoryCache();
        services.AddSingleton<IChatGptCache, ChatGptMemoryCache>();

        services.AddHttpClient();
        services.AddSingleton<IChatGptClientFactory, ChatGptClientFactory>(
            serviceProvider => new ChatGptClientFactory(serviceProvider, serviceProvider.GetRequiredService<IChatGptCache>(), deafultOptions)
        );

        return services;
    }
}
