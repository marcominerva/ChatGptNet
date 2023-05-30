using Microsoft.Extensions.Configuration;

namespace ChatGptNet.ServiceConfigurations;

/// <summary>
/// Contains configuration settings for Azure OpenAI service.
/// </summary>
internal class AzureChatGptServiceConfiguration : ChatGptServiceConfiguration
{
    private const string ApiVersion = "2023-03-15-preview";

    /// <summary>
    /// Gets or sets the name of the Azure OpenAI Resource.
    /// </summary>
    public string? ResourceName { get; set; }

    /// <summary>
    /// Gets or sets the authentication type for Azure OpenAI service.
    /// </summary>
    public AzureAuthenticationType AuthenticationType { get; set; }

    /// <summary>
    /// Creates a new instance of the <see cref="AzureChatGptServiceConfiguration"/> class.
    /// </summary>
    public AzureChatGptServiceConfiguration()
    {
    }

    public AzureChatGptServiceConfiguration(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        ResourceName = configuration.GetValue<string>("ResourceName");
        ArgumentNullException.ThrowIfNull(nameof(ResourceName));

        AuthenticationType = configuration.GetValue<string>("AuthenticationType")?.ToLowerInvariant() switch
        {
            "activedirectory" or "azureactivedirectory" or "azure" or "azuread" or "ad" => AzureAuthenticationType.ActiveDirectory,
            _ => AzureAuthenticationType.ApiKey  // API Key is the default.
        };
    }

    /// <inheritdoc />
    public override Uri GetServiceEndpoint(string? modelName)
    {
        ArgumentNullException.ThrowIfNull(nameof(modelName));

        var endpoint = new Uri($"https://{ResourceName}.openai.azure.com/openai/deployments/{modelName}/chat/completions?api-version={ApiVersion}");
        return endpoint;
    }

    /// <inheritdoc />
    public override IDictionary<string, string?> GetRequestHeaders()
    {
        var headers = new Dictionary<string, string?>();

        if (AuthenticationType == AzureAuthenticationType.ApiKey)
        {
            headers["api-key"] = ApiKey;
        }
        else
        {
            headers["Authorization"] = $"Bearer {ApiKey}";
        }

        return headers;
    }
}
