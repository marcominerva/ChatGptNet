using System.Net;
using ChatGptNet.Models;

namespace ChatGptNet.Exceptions;

/// <summary>
/// Represents errors that occur during API invocation.
/// </summary>
public class ChatGptException : HttpRequestException
{
    /// <summary>
    /// Gets the detailed error information.
    /// </summary>
    /// <seealso cref="ChatGptError"/>
    public ChatGptError Error { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatGptException"/> class with the specified <paramref name="error"/> details.
    /// </summary>
    /// <param name="error">The detailed error information</param>
    /// <param name="statusCode">The HTTP status code</param>
    /// <seealso cref="ChatGptError"/>
    /// <seealso cref="HttpRequestException"/>
    public ChatGptException(ChatGptError error, HttpStatusCode statusCode) : base(error.Message, null, statusCode)
    {
        Error = error;
    }
}
