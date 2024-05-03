using System.Text.Json.Serialization;

namespace ChatGptNet.Models;

/// <summary>
/// Represents a message content token with log probability information.
/// </summary>
public class ChatGptLogProbabilityContent
{
    /// <summary>
    /// Gets or sets the token.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the log probability of this token, if it is within the top 20 most likely tokens. Otherwise, the value -9999.0 is used to signify that the token is very unlikely.
    /// </summary>
    [JsonPropertyName("logprob")]
    public double LogProbality { get; set; }

    /// <summary>
    /// Gets or sets a list of integers representing the UTF-8 bytes representation of the token. Useful in instances where characters are represented by multiple tokens and their byte representations must be combined to generate the correct text representation. Can be <see langword="null"/> if there is no bytes representation for the token.
    /// </summary>
    public IEnumerable<byte>? Bytes { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of the most likely tokens and their log probability, at this token position. In rare cases, there may be fewer than the number of requested <see cref="ChatGptParameters.TopLogProbabilities"/> returned.
    /// </summary>
    /// <seealso cref="ChatGptParameters.TopLogProbabilities"/>
    [JsonPropertyName("top_logprobs")]
    public IEnumerable<ChatGptLogProbabilityContent>? TopLogProbabilities { get; set; }
}