using ChatGptNet.Exceptions;
using ChatGptNet.Models;
using ChatGptNet.ServiceConfigurations;

namespace ChatGptNet;

/// <summary>
/// Options class that provides settings for configuring ChatGPT.
/// </summary>
public class ChatGptOptions
{
    /// <summary>
    /// Gets or sets the configuration settings for accessing the service.
    /// </summary>
    /// <seealso cref="ChatGptServiceConfiguration"/>
    /// <seealso cref="OpenAIChatGptServiceConfiguration"/>
    /// <seealso cref="AzureChatGptServiceConfiguration"/>
    public ChatGptServiceConfiguration ServiceConfiguration { get; set; } = default!;

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
    /// Gets or sets the default model for chat completion. (default: <see cref="ChatGptModels.Gpt35Turbo"/>).
    /// </summary>
    /// <see cref="ChatGptModels"/>
    public string DefaultModel { get; set; } = ChatGptModels.Gpt35Turbo;

    /// <summary>
    ///  Gets or sets the default parameters for chat completion.
    /// </summary>
    /// <see cref="ChatGptParameters"/>
    public ChatGptParameters DefaultParameters { get; } = new();

    /// <summary>
    /// Gets or sets the user identification for chat completion, which can help OpenAI to monitor and detect abuse.
    /// </summary>
    /// <remarks>
    /// See <see href="https://platform.openai.com/docs/guides/safety-best-practices/end-user-ids">Safety best practices</see> for more information.
    /// </remarks>
    public string? User { get; set; }
}
