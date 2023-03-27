using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace ChatGptNet;

/// <summary>
/// Provides extension methods for adding ChatGPT support in NET applications.
/// </summary>
public static class ChatGptServiceCollectionExtensions
{
    /// <summary>
    /// Registers a <see cref="ChatGptClient"/> instance with the specified options.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="setupAction">The <see cref="Action{ChatGptOptions}"/> to configure the provided <see cref="ChatGptOptions"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>This method automatically adds a <see cref="MemoryCache"/> that is used to save chat messages for completion.</remarks>
    /// <seealso cref="ChatGptOptions"/>
    /// <see cref="MemoryCacheServiceCollectionExtensions.AddMemoryCache(IServiceCollection)"/>
    public static IServiceCollection AddChatGpt(this IServiceCollection services, Action<ChatGptOptions> setupAction)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(setupAction);

        var options = new ChatGptOptions();
        setupAction.Invoke(options);
        services.AddSingleton(options);

        AddChatGptCore(services);

        return services;
    }

    /// <summary>
    /// Registers a <see cref="ChatGptClient"/> instance using dynamic options.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="setupAction">The <see cref="Action{IServiceProvider, ChatGptOptions}"/> to configure the provided <see cref="ChatGptOptions"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>Use this this method if it is necessary to dynamically set options like <see cref="ChatGptOptions.ApiKey"/> (for example, using other services via dependency injection).
    /// This method automatically adds a <see cref="MemoryCache"/> that is used to save chat messages for completion.
    /// </remarks>
    /// <seealso cref="ChatGptOptions"/>
    /// <seealso cref="IServiceProvider"/>
    /// <see cref="MemoryCacheServiceCollectionExtensions.AddMemoryCache(IServiceCollection)"/>
    public static IServiceCollection AddChatGpt(this IServiceCollection services, Action<IServiceProvider, ChatGptOptions> setupAction)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(setupAction);

        var options = new ChatGptOptions();
        services.AddScoped(provider =>
        {
            setupAction.Invoke(provider, options);
            return options;
        });

        AddChatGptCore(services);

        return services;
    }

    private static void AddChatGptCore(IServiceCollection services)
    {
        services.AddMemoryCache();

        services.AddHttpClient<IChatGptClient, ChatGptClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.openai.com/v1/");
        });
    }
}
