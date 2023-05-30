using Microsoft.Extensions.DependencyInjection;

namespace ChatGptNet;

/// <inheritdoc/>
public class ChatGptBuilder : IChatGptBuilder
{
    /// <inheritdoc/>
    public IServiceCollection Services { get; }

    /// <inheritdoc/>
    public IHttpClientBuilder HttpClientBuilder { get; }

    internal ChatGptBuilder(IServiceCollection services, IHttpClientBuilder httpClientBuilder)
    {
        Services = services;
        HttpClientBuilder = httpClientBuilder;
    }
}
