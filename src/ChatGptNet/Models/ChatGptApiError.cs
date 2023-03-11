using System.Text.Json.Serialization;

namespace ChatGptNet.Models;

/// <summary>
/// Contains information about the error occurred while invoking API endpoints
/// </summary>
public class ChatGptApiError
{
    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error type
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
}

internal class ChatGptApiErrorRoot
{
    public ChatGptApiError Error { get; set; } = new();
}