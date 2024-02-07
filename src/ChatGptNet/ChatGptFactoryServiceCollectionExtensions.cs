using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChatGptNet.Models;
using ChatGptNet.ServiceConfigurations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ChatGptNet;
/// <summary>
/// Provides extension methods for adding ChatGPT Client Factory support in NET applications.
/// </summary>
public static class ChatGptFactoryServiceCollectionExtensions
{
    /// <summary>
    /// Registers a <see cref="ChatGptClientFactory"/> instance with the specified options.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="setupAction">The <see cref="Action{ChatGptOptions}"/> to configure the provided <see cref="ChatGptOptions"/>.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddChatGptClientFactory(this IServiceCollection services, Action<ChatGptOptionsBuilder> builder)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = new ChatGptOptionsBuilder();
        if (builder is not null)
            builder.Invoke(options);

        return AddChatGptClientFactoryCore(services, options);
    }

    /// <summary>
    /// Registers a <see cref="ChatGptClientFactory"/> instance.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddChatGptClientFactory(this IServiceCollection services)
    {
        return services.AddChatGptClientFactory(null);
    }

    private static IServiceCollection AddChatGptClientFactoryCore(this IServiceCollection services, ChatGptOptionsBuilder deafultOptions)
    {
        services.AddMemoryCache();
        services.AddSingleton<IChatGptCache, ChatGptMemoryCache>();

        var httpClientBuilder = services.AddHttpClient<IChatGptClientFactory, ChatGptClientFactory>();
        services.AddSingleton<IChatGptClientFactory, ChatGptClientFactory>(
            s => new ChatGptClientFactory(s, s.GetRequiredService<IChatGptCache>(), deafultOptions)
        );

        return services;
    }

    private static void SetMissingDefaults(ChatGptOptions options)
    {
        // If the provider is OpenAI and no default model has been specified, uses gpt-3.5-turbo by default.
        if (options.ServiceConfiguration is OpenAIChatGptServiceConfiguration && string.IsNullOrWhiteSpace(options.DefaultModel))
        {
            options.DefaultModel = OpenAIChatGptModels.Gpt35Turbo;
        }
    }
}

/// <summary>
/// Represents the default builder for configuring ChatGPT client factory.
/// </summary>
/// <seealso cref="IChatGptBuilder"/>"/>
public class ChatGptClientFactoryBuilder : IChatGptClientFactoryBuilder
{
    /// <summary>
    /// Gets the <see cref="IServiceCollection"/> where ChatGPT services are registered.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <inheritdoc/>
    /// <summary>
    /// Gets the <see cref="IHttpClientBuilder"/> used to configure the <see cref="HttpClient"/> used by ChatGPT.
    /// </summary>
    public IHttpClientBuilder HttpClientBuilder { get; }

    internal ChatGptClientFactoryBuilder(IServiceCollection services, IHttpClientBuilder httpClientBuilder)
    {
        Services = services;
        HttpClientBuilder = httpClientBuilder;
    }
}

/// <summary>
/// Represents a builder for configuring ChatGPT client factory.
/// </summary>
public interface IChatGptClientFactoryBuilder
{
    /// <summary>
    /// Gets the <see cref="IServiceCollection"/> where ChatGPT services are registered.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Gets the <see cref="IHttpClientBuilder"/> used to configure the <see cref="HttpClient"/> used by ChatGPT.
    /// </summary>
    IHttpClientBuilder HttpClientBuilder { get; }
}
