using Microsoft.Extensions.Configuration;

namespace ChatGptNet.ServiceConfigurations;

/// <summary>
/// Contains configuration settings for Azure OpenAI service.
/// </summary>
internal class AzureChatGptServiceConfiguration : ChatGptServiceConfiguration
{
    /// <summary>
    /// The default API version for Azure OpenAI service.
    /// </summary>
    public const string DefaultApiVersion = "2024-02-15-preview";

    /// <summary>
    /// Gets or sets the name of the Azure OpenAI Resource.
    /// </summary>
    public string? ResourceName { get; set; }

    /// <summary>
    /// Gets or sets the API version of the Azure OpenAI service (Default: 2024-02-15-preview).
    /// </summary>
    /// <remarks>
    /// Currently supported versions are:
    /// <list type = "bullet" >
    ///   <item>
    ///     <term>2023-05-15</term>
    ///     <description><see href="https://github.com/Azure/azure-rest-api-specs/blob/main/specification/cognitiveservices/data-plane/AzureOpenAI/inference/stable/2023-05-15/inference.json">Swagger spec</see></description>
    ///   </item>
    ///   <item>
    ///     <term>2023-06-01-preview</term>
    ///     <description><see href="https://github.com/Azure/azure-rest-api-specs/blob/main/specification/cognitiveservices/data-plane/AzureOpenAI/inference/preview/2023-06-01-preview/inference.json">Swagger spec</see></description>
    ///   </item>
    ///   <item>
    ///     <term>2024-02-15-preview</term>
    ///     <description><see href="https://github.com/Azure/azure-rest-api-specs/blob/main/specification/cognitiveservices/data-plane/AzureOpenAI/inference/preview/2024-02-15-preview/inference.json">Swagger spec</see></description>
    ///   </item>
    /// </list>
    /// </remarks>
    public string ApiVersion { get; set; } = DefaultApiVersion;

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
        ArgumentNullException.ThrowIfNull(ResourceName);

        ApiVersion = configuration.GetValue<string>("ApiVersion") ?? DefaultApiVersion;

        AuthenticationType = configuration.GetValue<string>("AuthenticationType")?.ToLowerInvariant() switch
        {
            "activedirectory" or "azureactivedirectory" or "azure" or "azuread" or "ad" => AzureAuthenticationType.ActiveDirectory,
            _ => AzureAuthenticationType.ApiKey  // API Key is the default.
        };
    }

    /// <inheritdoc />
    public override Uri GetChatCompletionEndpoint(string? modelName)
    {
        ArgumentNullException.ThrowIfNull(modelName);

        var endpoint = new Uri($"https://{ResourceName}.openai.azure.com/openai/deployments/{modelName}/chat/completions?api-version={ApiVersion}");
        return endpoint;
    }

    /// <inheritdoc />
    public override Uri GetEmbeddingEndpoint(string? modelName = null)
    {
        ArgumentNullException.ThrowIfNull(nameof(modelName));

        var endpoint = new Uri($"https://{ResourceName}.openai.azure.com/openai/deployments/{modelName}/embeddings?api-version={ApiVersion}");
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
