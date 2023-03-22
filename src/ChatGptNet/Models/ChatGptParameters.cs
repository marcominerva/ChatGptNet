using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ChatGptNet.Models;

/// <summary>
/// Represents chat completion parameters.
/// </summary>
public class ChatGptParameters
{
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
    /// <example>
    ///     <para>
    ///         The following is an example showing how to set the property from a string
    ///     </para>
    ///     <code>parameters.LogitBias = JsonNode.Parse("{\"50256\": -100}");</code>
    /// </example>
    /// <seealso cref="JsonNode.Parse(string, JsonNodeOptions?, System.Text.Json.JsonDocumentOptions)"/>
    /// <seealso cref="JsonNode.Parse(ref System.Text.Json.Utf8JsonReader, JsonNodeOptions?)"/>
    /// <seealso cref="JsonNode.Parse(ReadOnlySpan{byte}, JsonNodeOptions?, System.Text.Json.JsonDocumentOptions)"/>
    /// <seealso cref="JsonNode.Parse(Stream, JsonNodeOptions?, System.Text.Json.JsonDocumentOptions)"/>
    /// <remarks>
    /// Refer to https://platform.openai.com/docs/api-reference/chat/create#chat/create-logit_bias for more information
    /// </remarks>
    [JsonPropertyName("logit_bias")]
    public JsonNode? LogitBias { get; set; }
}