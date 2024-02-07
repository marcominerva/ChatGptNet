using ChatGptNet.Models;
using ChatGptNet.Models.Embeddings;
using ChatGptNet.ServiceConfigurations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatGptNet;

/// <summary>
/// Provides extension methods for adding ChatGPT support in .NET applications.
/// </summary>
public static class ChatGptServiceCollectionExtensions
{
    /// <summary>
    /// Registers a <see cref="ChatGptClient"/> instance with the specified options.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="builder">The <see cref="ChatGptOptionsBuilder"/> to configure options.</param>
    /// <param name="httpClientBuilder">The <see cref="IHttpClientBuilder"/> to configure the HTTP client used to make HTTP requests.</param>
    /// <returns>A <see cref="IChatGptBuilder"/> that can be used to further customize ChatGPT.</returns>
    /// <remarks>This method automatically adds a <see cref="MemoryCache"/> that is used to save conversation history for chat completion.
    /// It is possibile to use <see cref="IChatGptBuilderExtensions.WithCache{TImplementation}(IChatGptBuilder, ServiceLifetime)"/> to specify another cache implementation.
    /// </remarks>
    /// <seealso cref="ChatGptOptionsBuilder"/>
    /// <seealso cref="MemoryCacheServiceCollectionExtensions.AddMemoryCache(IServiceCollection)"/>
    /// <seealso cref="IChatGptBuilder"/>
    /// <seealso cref="IHttpClientBuilder"/>
    public static IChatGptBuilder AddChatGpt(this IServiceCollection services, Action<ChatGptOptionsBuilder> builder, Action<IHttpClientBuilder>? httpClientBuilder = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(builder);

        var options = new ChatGptOptionsBuilder();
        builder.Invoke(options);

        ArgumentNullException.ThrowIfNull(options.ServiceConfiguration);

        SetMissingDefaults(options);
        services.AddSingleton(options.Build());

        var chatGptBuilder = AddChatGptCore(services);
        httpClientBuilder?.Invoke(chatGptBuilder.HttpClientBuilder);

        return chatGptBuilder;
    }

    /// <summary>
    /// Registers a <see cref="ChatGptClient"/> instance reading configuration from the specified <see cref="IConfiguration"/> source.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> being bound.</param>
    /// <param name="sectionName">The name of the configuration section that holds ChatGPT settings.</param>
    /// <remarks>This method automatically adds a <see cref="MemoryCache"/> that is used to save conversation history for chat completion.
    /// It is possibile to use <see cref="IChatGptBuilderExtensions.WithCache{TImplementation}(IChatGptBuilder, ServiceLifetime)"/> to specify another cache implementation.
    /// </remarks>
    /// <seealso cref="ChatGptOptions"/>
    /// <seealso cref="IConfiguration"/>
    /// <seealso cref="MemoryCacheServiceCollectionExtensions.AddMemoryCache(IServiceCollection)"/>
    /// <seealso cref="IChatGptBuilder"/>
    public static IChatGptBuilder AddChatGpt(this IServiceCollection services, IConfiguration configuration, string sectionName)
        => services.AddChatGpt(configuration, sectionName, null);

    /// <summary>
    /// Registers a <see cref="ChatGptClient"/> instance reading configuration from the specified <see cref="IConfiguration"/> source, searching for the <em>ChatGPT</em> section.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> being bound.</param>
    /// <param name="httpClientBuilder">The <see cref="IHttpClientBuilder"/> to configure the HTTP client used to make HTTP requests.</param>
    /// <remarks>This method automatically adds a <see cref="MemoryCache"/> that is used to save conversation history for chat completion.
    /// It is possibile to use <see cref="IChatGptBuilderExtensions.WithCache{TImplementation}(IChatGptBuilder, ServiceLifetime)"/> to specify another cache implementation.
    /// </remarks>
    /// <seealso cref="ChatGptOptions"/>
    /// <seealso cref="IConfiguration"/>
    /// <seealso cref="MemoryCacheServiceCollectionExtensions.AddMemoryCache(IServiceCollection)"/>
    /// <seealso cref="IChatGptBuilder"/>
    /// <seealso cref="IHttpClientBuilder"/>
    public static IChatGptBuilder AddChatGpt(this IServiceCollection services, IConfiguration configuration, Action<IHttpClientBuilder>? httpClientBuilder = null)
        => services.AddChatGpt(configuration, "ChatGPT", httpClientBuilder);

    /// <summary>
    /// Registers a <see cref="ChatGptClient"/> instance reading configuration from the specified <see cref="IConfiguration"/> source.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> being bound.</param>
    /// <param name="sectionName">The name of the configuration section that holds ChatGPT settings.</param>
    /// <param name="httpClientBuilder">The <see cref="IHttpClientBuilder"/> to configure the HTTP client used to make HTTP requests.</param>/// <returns>A <see cref="IChatGptBuilder"/> that can be used to further customize ChatGPT.</returns>
    /// <remarks>This method automatically adds a <see cref="MemoryCache"/> that is used to save conversation history for chat completion.
    /// It is possibile to use <see cref="IChatGptBuilderExtensions.WithCache{TImplementation}(IChatGptBuilder, ServiceLifetime)"/> to specify another cache implementation.
    /// </remarks>
    /// <seealso cref="ChatGptOptions"/>
    /// <seealso cref="IConfiguration"/>
    /// <seealso cref="MemoryCacheServiceCollectionExtensions.AddMemoryCache(IServiceCollection)"/>
    /// <seealso cref="IChatGptBuilder"/>
    /// <seealso cref="IHttpClientBuilder"/>
    public static IChatGptBuilder AddChatGpt(this IServiceCollection services, IConfiguration configuration, string sectionName, Action<IHttpClientBuilder>? httpClientBuilder = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var options = new ChatGptOptionsBuilder();
        options.UseConfiguration(configuration, sectionName);

        SetMissingDefaults(options);
        services.AddSingleton(options.Build());

        var chatGptBuilder = AddChatGptCore(services);
        httpClientBuilder?.Invoke(chatGptBuilder.HttpClientBuilder);

        return chatGptBuilder;
    }

    /// <summary>
    /// Registers a <see cref="ChatGptClient"/> instance using dynamic options.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="builder">The <see cref="ChatGptOptionsBuilder"/> to configure options.</param>
    /// <param name="httpClientBuilder">The <see cref="IHttpClientBuilder"/> to configure the HTTP client used to make HTTP requests.</param>
    /// <returns>A <see cref="IChatGptBuilder"/> that can be used to further customize ChatGPT.</returns>
    /// <remarks>Use this this method if it is necessary to dynamically set options (for example, using other services via dependency injection).
    /// This method automatically adds a <see cref="MemoryCache"/> that is used to save conversation history for chat completion.
    /// It is possibile to use <see cref="IChatGptBuilderExtensions.WithCache{TImplementation}(IChatGptBuilder, ServiceLifetime)"/> to specify another cache implementation.
    /// </remarks>
    /// <seealso cref="ChatGptOptions"/>
    /// <seealso cref="IServiceProvider"/>
    /// <seealso cref="MemoryCacheServiceCollectionExtensions.AddMemoryCache(IServiceCollection)"/>
    /// <seealso cref="IChatGptBuilder"/>
    /// <seealso cref="IHttpClientBuilder"/>
    public static IChatGptBuilder AddChatGpt(this IServiceCollection services, Action<IServiceProvider, ChatGptOptionsBuilder> builder, Action<IHttpClientBuilder>? httpClientBuilder = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(builder);

        services.AddScoped(provider =>
        {
            var options = new ChatGptOptionsBuilder();
            builder.Invoke(provider, options);

            ArgumentNullException.ThrowIfNull(options.ServiceConfiguration);

            SetMissingDefaults(options);
            return options.Build();
        });

        var chatGptBuilder = AddChatGptCore(services);
        httpClientBuilder?.Invoke(chatGptBuilder.HttpClientBuilder);

        return chatGptBuilder;
    }

    private static ChatGptBuilder AddChatGptCore(IServiceCollection services)
    {
        // Uses MemoryCache by default.
        services.AddMemoryCache();
        services.AddSingleton<IChatGptCache, ChatGptMemoryCache>();

        var httpClientBuilder = services.AddHttpClient<IChatGptClient, ChatGptClient>();
        return new ChatGptBuilder(services, httpClientBuilder);
    }

    private static void SetMissingDefaults(ChatGptOptionsBuilder options)
    {
        if (options.ServiceConfiguration is OpenAIChatGptServiceConfiguration)
        {
            // If the provider is OpenAI and some default models are not specified, use the default ones.
            if (string.IsNullOrWhiteSpace(options.DefaultModel))
            {
                options.DefaultModel = OpenAIChatGptModels.Gpt35Turbo;
            }

            if (string.IsNullOrWhiteSpace(options.DefaultEmbeddingModel))
            {
                options.DefaultEmbeddingModel = OpenAIEmbeddingModels.TextEmbeddingAda002;
            }
        }
    }
}
