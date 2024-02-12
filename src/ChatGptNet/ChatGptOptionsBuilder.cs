using ChatGptNet.Exceptions;
using ChatGptNet.Models;
using ChatGptNet.Models.Embeddings;
using ChatGptNet.ServiceConfigurations;

namespace ChatGptNet;

/// <summary>
/// Builder class to define settings for configuring ChatGPT.
/// </summary>
public class ChatGptOptionsBuilder
{
    /// <summary>
    /// Gets or sets the configuration settings for accessing the service.
    /// </summary>
    /// <seealso cref="ChatGptServiceConfiguration"/>
    /// <seealso cref="OpenAIChatGptServiceConfiguration"/>
    /// <seealso cref="AzureChatGptServiceConfiguration"/>
    internal ChatGptServiceConfiguration ServiceConfiguration { get; set; } = default!;

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
    /// <seealso cref="ChatGptException"/>
    /// <seealso cref="ChatGptResponse"/>
    public bool ThrowExceptionOnError { get; set; } = true;

    /// <summary>
    /// Gets or sets the default model for chat completion. (default: <see cref="OpenAIChatGptModels.Gpt35Turbo"/> when the provider is <see cref="OpenAIChatGptServiceConfiguration"> OpenAI</see>).
    /// </summary>
    /// <seealso cref="OpenAIChatGptModels"/>
    /// <seealso cref="OpenAIChatGptServiceConfiguration"/>
    public string? DefaultModel { get; set; }

    /// <summary>
    /// Gets or sets the default model for embeddings. (default: <see cref="OpenAIEmbeddingModels.TextEmbeddingAda002"/> when the provider is <see cref="OpenAIChatGptServiceConfiguration"> OpenAI</see>).
    /// </summary>
    /// <seealso cref="OpenAIEmbeddingModels"/>
    /// <seealso cref="OpenAIChatGptServiceConfiguration"/>
    public string? DefaultEmbeddingModel { get; set; }

    /// <summary>
    ///  Gets or sets the default parameters for chat completion.
    /// </summary>
    /// <see cref="ChatGptParameters"/>
    public ChatGptParameters? DefaultParameters { get; set; } = new();

    /// <summary>
    ///  Gets or sets the default parameters for embeddings.
    /// </summary>
    /// <see cref="EmbeddingParameters"/>
    public EmbeddingParameters DefaultEmbeddingParameters { get; set; } = new();

    /// <summary>
    /// Gets or sets the user identification for chat completion, which can help OpenAI to monitor and detect abuse.
    /// </summary>
    /// <remarks>
    /// See <see href="https://platform.openai.com/docs/guides/safety-best-practices/end-user-ids">Safety best practices</see> for more information.
    /// </remarks>
    public string? User { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatGptOptionsBuilder"/> class.
    /// </summary>
    public ChatGptOptionsBuilder()
    {
    }

    internal ChatGptOptionsBuilder(ChatGptOptionsBuilder source)
    {
        MessageLimit = source.MessageLimit;
        DefaultModel = source.DefaultModel;
        DefaultEmbeddingModel = source.DefaultEmbeddingModel;

        DefaultParameters = new()
        {
            FrequencyPenalty = source.DefaultParameters?.FrequencyPenalty,
            MaxTokens = source.DefaultParameters?.MaxTokens,
            PresencePenalty = source.DefaultParameters?.PresencePenalty,
            ResponseFormat = source.DefaultParameters?.ResponseFormat,
            TopP = source.DefaultParameters?.TopP,
            Temperature = source.DefaultParameters?.Temperature,
            Seed = source.DefaultParameters?.Seed
        };

        DefaultEmbeddingParameters = new()
        {
            Dimensions = source.DefaultEmbeddingParameters.Dimensions
        };

        MessageExpiration = source.MessageExpiration;
        ThrowExceptionOnError = source.ThrowExceptionOnError;

        ServiceConfiguration = source.ServiceConfiguration switch
        {
            OpenAIChatGptServiceConfiguration openAI => new OpenAIChatGptServiceConfiguration
            {
                ApiKey = openAI.ApiKey,
                Organization = openAI.Organization
            },
            AzureChatGptServiceConfiguration azure => new AzureChatGptServiceConfiguration
            {
                ResourceName = azure.ResourceName,
                ApiKey = azure.ApiKey,
                ApiVersion = azure.ApiVersion,
                AuthenticationType = azure.AuthenticationType
            },
            _ => throw new ArgumentException("Invalid service configuration type.")
        };

        User = source.User;
    }

    internal ChatGptOptions Build()
        => new()
        {
            MessageLimit = MessageLimit,
            DefaultModel = DefaultModel,
            DefaultEmbeddingModel = DefaultEmbeddingModel,
            DefaultParameters = DefaultParameters ?? new(),
            DefaultEmbeddingParameters = DefaultEmbeddingParameters ?? new(),
            MessageExpiration = MessageExpiration,
            ThrowExceptionOnError = ThrowExceptionOnError,
            ServiceConfiguration = ServiceConfiguration,
            User = User
        };
}
