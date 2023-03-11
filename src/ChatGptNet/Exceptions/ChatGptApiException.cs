using System.Net;
using ChatGptNet.Models;

namespace ChatGptNet.Exceptions;

/// <summary>
/// Represents errors that occur during API invocation.
/// </summary>
public class ChatGptApiException : HttpRequestException
{
    /// <summary>
    /// Gets the detailed error information.
    /// </summary>
    /// <seealso cref="ChatGptApiError"/>
    public ChatGptApiError Error { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatGptApiException"/> class with the specified <paramref name="error"/> details.
    /// </summary>
    /// <param name="error">The detailed error information</param>
    /// <param name="statusCode">The HTTP status code</param>
    /// <seealso cref="ChatGptApiError"/>
    public ChatGptApiException(ChatGptApiError error, HttpStatusCode statusCode) : base(error.Message, null, statusCode)
    {
        Error = error;
    }
}
