using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace ChatGptNet;

/// <summary>
/// Provides extension methods for adding ChatGPT support in NET applications.
/// </summary>
public static class ChatGptServiceCollectionEstensions
{
    /// <summary>
    /// Registers <see cref="ChatGptClient"/> with the specified options.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="setupAction">The <see cref="Action{ChatGptOptions}"/> to configure the provided <see cref="ChatGptOptions"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>This methods automatically adds a <see cref="MemoryCache"/> that is used to save chat messages for completion.</remarks>
    /// <seealso cref="ChatGptOptions"/>
    /// <see cref="MemoryCacheServiceCollectionExtensions.AddMemoryCache(IServiceCollection)"/>
    public static IServiceCollection AddChatGpt(this IServiceCollection services, Action<ChatGptOptions> setupAction)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(setupAction);

        var options = new ChatGptOptions();
        setupAction.Invoke(options);
        services.AddSingleton(options);

        services.AddMemoryCache();

        services.AddHttpClient<IChatGptClient, ChatGptClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.openai.com/v1/");
            client.DefaultRequestHeaders.Authorization = new("Bearer", options.ApiKey);

            if (!string.IsNullOrWhiteSpace(options.Organization))
            {
                client.DefaultRequestHeaders.Add("OpenAI-Organization", options.Organization);
            }
        });

        return services;
    }
}
