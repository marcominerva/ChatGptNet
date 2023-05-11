using Microsoft.Extensions.Configuration;

namespace ChatGptNet.ServiceConfigurations;

/// <summary>
/// Contains configuration settings for OpenAI services.
/// </summary>
public class OpenAIChatGptServiceConfiguration : ChatGptServiceConfiguration
{
    /// <summary>
    /// Gets or sets a value that determines the organization the user belongs to.
    /// </summary>
    /// <remarks>For users who belong to multiple organizations, you can pass a header to specify which organization is used for an API request. Usage from these API requests will count against the specified organization's subscription quota.</remarks>
    public string? Organization { get; set; }

    /// <summary>
    /// Creates a new instance of the <see cref="OpenAIChatGptServiceConfiguration"/> class.
    /// </summary>
    public OpenAIChatGptServiceConfiguration()
    {
    }

    internal OpenAIChatGptServiceConfiguration(IConfiguration configuration)
    {
        Organization = configuration.GetValue<string>("Organization");
    }

    /// <inheritdoc />
    public override Uri GetServiceEndpoint(string? modelName) => new("https://api.openai.com/v1/chat/completions");

    /// <inheritdoc />
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
