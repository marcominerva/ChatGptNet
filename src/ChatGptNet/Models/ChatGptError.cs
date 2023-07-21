using System.Text.Json.Serialization;

namespace ChatGptNet.Models;

/// <summary>
/// Contains information about the error occurred while invoking the service.
/// </summary>
/// <remarks>
/// See <see href="https://platform.openai.com/docs/guides/error-codes">Error codes</see> for more information.
/// </remarks>
public class ChatGptError
{
    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error type.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the parameter that caused the error.
    /// </summary>
    [JsonPropertyName("param")]
    public string? Parameter { get; set; }

    /// <summary>
    /// Gets or sets the error code.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Gets or sets the HTTP status code of the error.
    /// </summary>
    [JsonPropertyName("status")]
    public int StatusCode { get; set; }

    /// <summary>
    /// Contains further details about the error.
    /// </summary>
    public ChatGptInnerError? InnerError { get; set; }
}