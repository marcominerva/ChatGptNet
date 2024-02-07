using System.Text.Json.Serialization;

namespace ChatGptNet.Models;

/// <summary>
/// Represents a request for a chat completions.
/// </summary>
/// <remarks>
/// See <see href="https://platform.openai.com/docs/api-reference/chat/create">Create chat completion (OpenAI)</see> or <see href="https://learn.microsoft.com/azure/cognitive-services/openai/reference#chat-completions">Chat Completions (Azure)</see> for more information.
/// </remarks>
internal class ChatGptRequest
{
    /// <summary>
    /// Gets or sets the ID of the model to use.
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Gets or sets the messages to generate chat completions for.
    /// </summary>
    /// <seealso cref="ChatGptMessage"/>
    public IEnumerable<ChatGptMessage> Messages { get; set; } = Enumerable.Empty<ChatGptMessage>();

    /// <summary>
    /// Gets or sets a list of functions the model may generate JSON inputs for.
    /// </summary>
    /// <seealso cref="ChatGptFunction"/>
    /// <seealso cref="FunctionCall"/>
    public IEnumerable<ChatGptFunction>? Functions { get; set; }

    /// <summary>
    /// Controls how the model responds to function calls. <em>none</em> means the model does not call a function, and responds to the end-user. <em>auto</em> means the model can pick between an end-user or calling a function. Specifying a particular function name forces the model to call that function.
    /// </summary>
    /// <remarks>
    /// <em>none</em> is the default when no functions are present. <em>auto</em> is the default if functions are present.
    /// </remarks>
    /// <seealso cref="ChatGptFunction"/>
    [JsonPropertyName("function_call")]
    public object? FunctionCall { get; set; }

    /// <summary>
    /// Gets or sets a value that specify if response will be sent in streaming as partial message deltas.
    /// </summary>
    public bool Stream { get; set; }

    /// <summary>
    /// Gets or sets what sampling temperature to use, between 0 and 2. Higher values like 0.8 will make the output more random, while lower values like 0.2 will make it more focused and deterministic (default: 1).
    /// </summary>
    /// <remarks>
    /// It is generally recommend altering this value or <see cref="TopP"/> but not both.
    /// </remarks>
    /// <seealso cref="TopP"/>
    public double? Temperature { get; set; }

    /// <summary>
    /// Gets or sets an alternative to sampling with temperature, called nucleus sampling, where the model considers the results of the tokens with <see cref="TopP"/> probability mass. So 0.1 means only the tokens comprising the top 10% probability mass are considered (default: 1).
    /// </summary>
    /// <remarks>
    /// It is generally recommend altering this value or <see cref="Temperature"/> but not both.
    /// </remarks>
    /// <seealso cref="Temperature"/>
    [JsonPropertyName("top_p")]
    public double? TopP { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of tokens to generate in the chat completion. The total length of input tokens and generated tokens is limited by the model's context length.
    /// </summary>
    [JsonPropertyName("max_tokens")]
    public int? MaxTokens { get; set; }

    /// <summary>
    /// Gets or sets the presence penalties for chat completion. Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in the text so far, increasing the model's likelihood to talk about new topics (default: 0).
    /// </summary>
    /// <remarks>
    /// See <see href="https://platform.openai.com/docs/api-reference/parameter-details">Parameter details</see> for more information.
    /// </remarks>
    [JsonPropertyName("presence_penalty")]
    public double? PresencePenalty { get; set; }

    /// <summary>
    /// Gets or sets the frequency penalties for chat completion. Number between -2.0 and 2.0. Positive values penalize new tokens based on their existing frequency in the text so far, decreasing the model's likelihood to repeat the same line verbatim (default: 0).
    /// </summary>
    /// <remarks>
    /// See <see href="https://platform.openai.com/docs/api-reference/parameter-details">Parameter details</see> for more information.
    /// </remarks>
    [JsonPropertyName("frequency_penalty")]
    public double? FrequencyPenalty { get; set; }

    /// <summary>
    /// Gets or sets the user identification for chat completion, which can help OpenAI to monitor and detect abuse.
    /// </summary>
    /// <remarks>
    /// See <see href="https://platform.openai.com/docs/guides/safety-best-practices/end-user-ids">Safety best practices</see> for more information.
    /// </remarks>
    public string? User { get; set; }
}