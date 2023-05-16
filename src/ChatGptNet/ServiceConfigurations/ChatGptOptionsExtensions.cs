namespace ChatGptNet.ServiceConfigurations;

/// <summary>
/// Provides extensions to configure settings for accessing ChatGPT service
/// </summary>
public static class ChatGptOptionsExtensions
{
    /// <summary>
    /// Configures OpenAI settings.
    /// </summary>
    /// <param name="options">The <see cref="ChatGptOptions"/> object to configure.</param>
    /// <param name="apiKey">The API Key to access the service.</param>
    /// <param name="organization">A value that determines the organization the user belongs to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="apiKey"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// See <see href="https://help.openai.com/en/articles/4936850-where-do-i-find-my-secret-api-key">Where do I find my Secret API Key?</see> for more information about the API Key.
    /// If an organization is specified, usage from these API requests will count against the specified organization's subscription quota.
    /// </remarks>
    /// <seealso cref="ChatGptOptions"/>
    public static ChatGptOptions UseOpenAI(this ChatGptOptions options, string apiKey, string? organization = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(apiKey);

        options.ServiceConfiguration = new OpenAIChatGptServiceConfiguration
        {
            ApiKey = apiKey,
            Organization = organization
        };

        return options;
    }

    /// <summary>
    /// Configures Azure OpenAI Service settings.
    /// </summary>
    /// <param name="options">The <see cref="ChatGptOptions"/> object to configure.</param>
    /// <param name="resourceName">The name of the Azure OpenAI Resource.</param>
    /// <param name="apiKey">The access key to access the service.</param>
    /// <param name="authenticationType">Specify if <paramref name="apiKey"/> is an actual API Key or an Azure Active Directory token.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="resourceName"/> or <paramref name="apiKey"/> are <see langword="null"/>.</exception>
    /// <remarks>
    /// See <see href="https://learn.microsoft.com/azure/cognitive-services/openai/reference#authentication">Azure OpenAI Service Authentication</see> and <see href="https://learn.microsoft.com/azure/cognitive-services/openai/how-to/managed-identity">Authenticating with Azure Active Directory</see> for more information about authentication.
    /// </remarks>
    /// <seealso cref="ChatGptOptions"/>
    public static ChatGptOptions UseAzure(this ChatGptOptions options, string resourceName, string apiKey, AzureAuthenticationType authenticationType = AzureAuthenticationType.ApiKey)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(resourceName);
        ArgumentNullException.ThrowIfNull(apiKey);

        options.ServiceConfiguration = new AzureChatGptServiceConfiguration
        {
            ResourceName = resourceName,
            ApiKey = apiKey,
            AuthenticationType = authenticationType
        };

        return options;
    }
}
