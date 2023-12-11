using System.Text.Json.Serialization;

namespace ChatGptNet.Models;

/// <summary>
/// Represents chat completion parameters.
/// </summary>
/// <remarks>
/// See <see href="https://platform.openai.com/docs/api-reference/chat/create">Create chat completion</see> for more information.
/// </remarks>
public class ChatGptParameters
{
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
    public double? TopP { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of tokens to generate in the chat completion. The total length of input tokens and generated tokens is limited by the model's context length.
    /// </summary>
    public int? MaxTokens { get; set; }

    /// <summary>
    /// Gets or sets the presence penalties for chat completion. Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in the text so far, increasing the model's likelihood to talk about new topics (default: 0).
    /// </summary>
    /// <remarks>
    /// See <see href="https://platform.openai.com/docs/api-reference/parameter-details">Parameter details</see> for more information.
    /// </remarks>
    public double? PresencePenalty { get; set; }

    /// <summary>
    /// Gets or sets the frequency penalties for chat completion. Number between -2.0 and 2.0. Positive values penalize new tokens based on their existing frequency in the text so far, decreasing the model's likelihood to repeat the same line verbatim (default: 0).
    /// </summary>
    /// <remarks>
    /// See <see href="https://platform.openai.com/docs/api-reference/parameter-details">Parameter details</see> for more information.
    /// </remarks>
    public double? FrequencyPenalty { get; set; }

    /// <summary>
    /// An object specifying the format that the model must output. Used to enable JSON mode.
    /// </summary>
    /// <seealso cref="ChatGptResponseFormat"/>
    [JsonPropertyName("response_format")]
    public ChatGptResponseFormat? ResponseFormat { get; set; }
}