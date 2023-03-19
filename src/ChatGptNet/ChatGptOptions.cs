using ChatGptNet.Exceptions;
using ChatGptNet.Models;

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
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of messages to use for chat completion (default: 10).
    /// </summary>
    public int MessageLimit { get; set; } = 10;

    /// <summary>
    /// Gets or sets the expiration for cached conversation messages (default: 1 hour).
    /// </summary>
    public TimeSpan MessageExpiration { get; set; } = TimeSpan.FromHours(1);

    /// <summary>
    /// Gets or sets a value that determines whether to throw a <see cref="ChatGptException"/> when an error occurred (default: <see langword="true"/>). If this property is set to <see langword="false"></see>, API errors are returned in the <see cref="ChatGptResponse"/> object.
    /// </summary>
    /// <see cref="ChatGptException"/>
    /// <seealso cref="ChatGptResponse"/>
    public bool ThrowExceptionOnError { get; set; } = true;

    /// <summary>
    /// Gets or sets a value that determines the organization the user belongs to.
    /// </summary>
    /// <remarks>For users who belong to multiple organizations, you can pass a header to specify which organization is used for an API request. Usage from these API requests will count against the specified organization's subscription quota.</remarks>
    public string? Organization { get; set; }
}
