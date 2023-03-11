namespace ChatGptNet;

/// <summary>
/// Options class that provides settings for configuring ChatGPT
/// </summary>
public class ChatGptOptions
{
    /// <summary>
    /// Gets or sets the API Key to access the service.
    /// </summary>
    /// <remarks>
    /// Refer to https://help.openai.com/en/articles/4936850-where-do-i-find-my-secret-api-key for more information.
    /// </remarks>
    public string ApiKey { get; set; } = null!;

    /// <summary>
    /// Gets or sets the maximum number of messages to use for chat completion (default: 10).
    /// </summary>
    public int MessageLimit { get; set; } = 10;
}
