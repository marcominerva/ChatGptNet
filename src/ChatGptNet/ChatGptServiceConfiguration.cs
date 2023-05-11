using Microsoft.Extensions.Configuration;

namespace ChatGptNet;

public abstract class ChatGptServiceConfiguration
{
    /// <summary>
    /// Gets or sets the API Key to access the service.
    /// </summary>
    public string? ApiKey { get; set; }

    public abstract Uri GetServiceEndpoint(string? modelName = null);

    public abstract IDictionary<string, string?> GetRequestHeaders();

    internal static ChatGptServiceConfiguration Create(IConfiguration configuration)
    {
        ChatGptServiceConfiguration serviceConfiguration = configuration.GetValue<ChatGptServiceType>("ServiceType") switch
        {
            ChatGptServiceType.OpenAI => new OpenAiChatGptServiceConfiguration(configuration),
            ChatGptServiceType.Azure => new AzureChatGptServiceConfiguration(configuration),
            _ => throw new ArgumentException("ServiceType")
        };

        serviceConfiguration.ApiKey = configuration.GetValue<string>("ApiKey");

        return serviceConfiguration;
    }
}

public enum ChatGptServiceType
{
    OpenAI,
    Azure
}

public class OpenAiChatGptServiceConfiguration : ChatGptServiceConfiguration
{
    /// <summary>
    /// Gets or sets a value that determines the organization the user belongs to.
    /// </summary>
    /// <remarks>For users who belong to multiple organizations, you can pass a header to specify which organization is used for an API request. Usage from these API requests will count against the specified organization's subscription quota.</remarks>
    public string? Organization { get; set; }

    public OpenAiChatGptServiceConfiguration()
    {
    }

    internal OpenAiChatGptServiceConfiguration(IConfiguration configuration)
    {
        Organization = configuration.GetValue<string>("Organization");
    }

    public override Uri GetServiceEndpoint(string? modelName) => new("https://api.openai.com/v1/chat/completions");

    public override IDictionary<string, string?> GetRequestHeaders()
    {
        var headers = new Dictionary<string, string?>
        {
            ["Authorization"] = $"Bearer {ApiKey}"
        };

        if (!string.IsNullOrWhiteSpace(Organization))
        {
            headers.Add("OpenAI-Organization", Organization);
        }

        return headers;
    }
}

public class AzureChatGptServiceConfiguration : ChatGptServiceConfiguration
{
    private const string ApiVersion = "2023-03-15-preview";

    public string? ResourceName { get; set; }

    public AzureChatGptServiceConfiguration()
    {
    }

    internal AzureChatGptServiceConfiguration(IConfiguration configuration)
    {
        ResourceName = configuration.GetValue<string>("ResourceName");
    }

    public override Uri GetServiceEndpoint(string? modelName)
    {
        ArgumentNullException.ThrowIfNull(nameof(modelName));

        var endpoint = new Uri($"https://{ResourceName}.openai.azure.com/openai/deployments/{modelName}/chat/completions?api-version={ApiVersion}");
        return endpoint;
    }

    public override IDictionary<string, string?> GetRequestHeaders()
    {
        var headers = new Dictionary<string, string?>
        {
            ["api-key"] = ApiKey
        };

        return headers;
    }
}