using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ChatGptNet.Models;

/// <summary>
/// Represents a request for a chat completions.
/// </summary>
public class ChatGptRequest
{
    /// <summary>
    /// Gets or sets the ID of the model to use. Currently, only <em>gpt-3.5-turbo</em> and <em>gpt-3.5-turbo-0301</em> are supported.
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the messages to generate chat completions for.
    /// </summary>
    /// <seealso cref="ChatGptMessage"/>
    public ChatGptMessage[] Messages { get; set; } = Array.Empty<ChatGptMessage>();

    /// <summary>
    /// Gets or sets a value that specify if response will be sent in streaming as partial message deltas.
    /// </summary>
    public bool Stream { get; set; }

    /// <summary>
    /// Gets or sets the temperature option for chat completion.
    /// </summary>
    /// <remarks>
    /// Refer to https://platform.openai.com/docs/api-reference/chat/create#chat/create-temperature for more information
    /// </remarks>
    public double? Temperature { get; set; }

    /// <summary>
    /// Gets or sets the top_p option for chat completion.
    /// </summary>
    /// <remarks>
    /// Refer to https://platform.openai.com/docs/api-reference/chat/create#chat/create-top_p for more information
    /// </remarks>
    [JsonPropertyName("top_p")]
    public double? TopP { get; set; }

    /// <summary>
    /// Gets or sets the n option for chat completion.
    /// </summary>
    /// <remarks>
    /// Refer to https://platform.openai.com/docs/api-reference/chat/create#chat/create-n for more information
    /// </remarks>
    public int? N { get; set; }

    /// <summary>
    /// Gets or sets the max_tokens option for chat completion.
    /// </summary>
    /// <remarks>
    /// Refer to https://platform.openai.com/docs/api-reference/chat/create#chat/create-max_tokens for more information
    /// </remarks>
    [JsonPropertyName("max_tokens")]
    public int? MaxTokens { get; set; }

    /// <summary>
    /// Gets or sets the presence_penalty option for chat completion.
    /// </summary>
    /// <remarks>
    /// Refer to https://platform.openai.com/docs/api-reference/chat/create#chat/create-presence_penalty for more information
    /// </remarks>
    [JsonPropertyName("presence_penalty")]
    public double? PresencePenalty { get; set; }

    /// <summary>
    /// Gets or sets the frequency_penalty option for chat completion.
    /// </summary>
    /// <remarks>
    /// Refer to https://platform.openai.com/docs/api-reference/chat/create#chat/create-frequency_penalty for more information
    /// </remarks>
    [JsonPropertyName("frequency_penalty")]
    public double? FrequencyPenalty { get; set; }

    /// <summary>
    /// Gets or sets the logit_bias option for chat completion.
    /// </summary>
    /// <remarks>
    /// Refer to https://platform.openai.com/docs/api-reference/chat/create#chat/create-logit_bias for more information
    /// </remarks>
    [JsonPropertyName("logit_bias")]
    public JsonNode? LogitBias { get; set; }
}
