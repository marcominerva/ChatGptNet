using Microsoft.Extensions.Configuration;

namespace ChatGptNet.ServiceConfigurations;

/// <summary>
/// Provides configuration properties for ChatGPT service.
/// </summary>
internal abstract class ChatGptServiceConfiguration
{
    /// <summary>
    /// Gets or sets the API Key to access the service.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Returns the <see cref="Uri"/> that provides chat completion responses.
    /// </summary>
    /// <param name="modelName">The name of the model for chat completion.</param>
    /// <returns>The <see cref="Uri"/> for chat completion.</returns>
    public abstract Uri GetChatCompletionEndpoint(string? modelName = null);

    /// <summary>
    /// Returns the <see cref="Uri"/> that generates embeddings.
    /// </summary>
    /// <param name="modelName">The name of the model for embeddings.</param>
    /// <returns>The <see cref="Uri"/> for generating embeddings.</returns>
    public abstract Uri GetEmbeddingEndpoint(string? modelName = null);

    /// <summary>
    /// Returns the headers that are required by the service to complete the request.
    /// </summary>
    /// <returns>The collection of headers.</returns>
    public abstract IDictionary<string, string?> GetRequestHeaders();

    internal static ChatGptServiceConfiguration Create(IConfiguration configuration)
    {
        ChatGptServiceConfiguration serviceConfiguration = configuration.GetValue<string>("Provider")?.ToLowerInvariant() switch
        {
            "azure" => new AzureChatGptServiceConfiguration(configuration),
            _ => new OpenAIChatGptServiceConfiguration(configuration),  // OpenAI is the default.
        };

        serviceConfiguration.ApiKey = configuration.GetValue<string>("ApiKey");

        return serviceConfiguration;
    }
}