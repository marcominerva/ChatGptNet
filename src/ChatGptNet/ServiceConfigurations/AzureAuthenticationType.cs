namespace ChatGptNet.ServiceConfigurations;

/// <summary>
/// Enumerates the available Azure authentication types for OpenAI service.
/// </summary>
/// <remarks>
/// See <see href="https://learn.microsoft.com/azure/cognitive-services/openai/reference#authentication">Azure OpenAI Service Authentication</see> and <see href="https://learn.microsoft.com/azure/cognitive-services/openai/how-to/managed-identity">Authenticating with Azure Active Directory</see> for more information about authentication.
/// </remarks>
public enum AzureAuthenticationType
{
    /// <summary>
    /// Authenticates using an API key.
    /// </summary>
    ApiKey,

    /// <summary>
    /// Authenticates using Azure Active Directory.
    /// </summary>
    ActiveDirectory
}
