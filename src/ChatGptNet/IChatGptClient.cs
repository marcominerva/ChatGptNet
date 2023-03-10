using ChatGptNet.Models;

namespace ChatGptNet;

public interface IChatGptClient
{
    Task<Response?> AskAsync(string message, CancellationToken cancellationToken = default);
}
