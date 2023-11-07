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
    /// Gets or sets a list of tools the model may call.
    /// </summary>
    /// <seealso cref="ChatGptTool"/>
    /// <seealso cref="ChatGptFunction"/>
    /// <seealso cref="ToolChoice"/>
    public IEnumerable<ChatGptTool>? Tools { get; set; }

    /// <summary>
    /// Controls which (if any) function is called by the model.
    /// </summary>
    /// <remarks>
    /// <list type = "bullet" >
    ///   <item>
    ///     <term><see cref="ChatGptToolChoices.None"/></term>
    ///     <description>Model will not call a function and instead generates a message.</description>
    ///   </item>
    ///   <item>
    ///     <term><see cref="ChatGptToolChoices.Auto"/></term>
    ///     <description>The model can pick between generating a message or calling a function</description>
    ///   </item>
    ///   <item>
    ///     <term><em>function_name</em></term>
    ///     <description>Specifying a particular function name forces the model to call that function.</description>
    ///   </item>
    /// </list>
    /// <see cref="ChatGptToolChoices.None"/> is the default when no functions are present. <see cref="ChatGptToolChoices.None"/> is the default if functions are present.
    /// </remarks>
    /// <seealso cref="ChatGptFunction"/>
    [JsonPropertyName("tool_choice")]
    public object? ToolChoice { get; set; }

    /// <summary>
    /// Gets or sets a value that specify if response will be sent in streaming as partial message deltas.
    /// </summary>
    public bool Stream { get; set; }

    /// <summary>
    /// If specified, the system will make a best effort to sample deterministically, such that repeated requests with the same seed and parameters should return the same result.
    /// </summary>
    /// <remarks>
    /// Determinism is not guaranteed, and you should refer to the <see cref="ChatGptResponse.SystemFingerprint"/> response parameter to monitor changes in the backend.
    /// </remarks>
    /// <seealso cref="ChatGptResponse.SystemFingerprint"/>
    public int? Seed { get; set; }

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
    /// Gets or sets the user identification for chat completion, which can help to monitor and detect abuse.
    /// </summary>
    /// <remarks>
    /// See <see href="https://platform.openai.com/docs/guides/safety-best-practices/end-user-ids">Safety best practices</see> for more information.
    /// </remarks>
    public string? User { get; set; }

    /// <summary>
    /// An object specifying the format that the model must output. Used to enable JSON mode.
    /// </summary>
    /// <seealso cref="ChatGptResponseFormat"/>
    [JsonPropertyName("response_format")]
    public ChatGptResponseFormat? ResponseFormat { get; set; }
}