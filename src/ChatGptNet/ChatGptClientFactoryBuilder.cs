using Microsoft.Extensions.DependencyInjection;

namespace ChatGptNet;

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
