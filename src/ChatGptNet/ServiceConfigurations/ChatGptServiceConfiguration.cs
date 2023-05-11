using Microsoft.Extensions.Configuration;

namespace ChatGptNet.ServiceConfigurations;

/// <summary>
/// Provides configuration properties for Chat GPT service.
/// </summary>
public abstract class ChatGptServiceConfiguration
{
    /// <summary>
    /// Gets or sets the API Key to access the service.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Returns the <see cref="Uri"/> that provides chat completion responses.
    /// </summary>
    /// <param name="modelName">The name of the model for chat completion.</param>
    /// <returns>The <see cref="Uri"/> of the service.</returns>
    public abstract Uri GetServiceEndpoint(string? modelName = null);

    /// <summary>
    /// Returns the headers that are required by the service to complete the request.
    /// </summary>
    /// <returns>The collection of headers.</returns>
    public abstract IDictionary<string, string?> GetRequestHeaders();

    internal static ChatGptServiceConfiguration Create(IConfiguration configuration)
    {
        ChatGptServiceConfiguration serviceConfiguration = configuration.GetValue<ChatGptServiceType>("Service") switch
        {
            ChatGptServiceType.OpenAI => new OpenAIChatGptServiceConfiguration(configuration),
            ChatGptServiceType.Azure => new AzureChatGptServiceConfiguration(configuration),
            _ => throw new ArgumentException("Service")
        };

        serviceConfiguration.ApiKey = configuration.GetValue<string>("ApiKey");

        return serviceConfiguration;
    }
}