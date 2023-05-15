using Microsoft.Extensions.Configuration;

namespace ChatGptNet.ServiceConfigurations;

/// <summary>
/// Contains configuration settings for Azure OpenAI services.
/// </summary>
public class AzureChatGptServiceConfiguration : ChatGptServiceConfiguration
{
    private const string ApiVersion = "2023-03-15-preview";

    /// <summary>
    /// Gets or sets the name of the Azure OpenAI Resource.
    /// </summary>
    public string? ResourceName { get; set; }

    /// <summary>
    /// Creates a new instance of the <see cref="AzureChatGptServiceConfiguration"/> class.
    /// </summary>
    public AzureChatGptServiceConfiguration()
    {
    }

    internal AzureChatGptServiceConfiguration(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        ResourceName = configuration.GetValue<string>("ResourceName");
        ArgumentNullException.ThrowIfNull(nameof(ResourceName));
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
        var headers = new Dictionary<string, string?>
        {
            ["api-key"] = ApiKey
        };

        return headers;
    }
}