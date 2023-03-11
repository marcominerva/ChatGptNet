using System.Net.Http.Json;
using ChatGptNet.Exceptions;
using ChatGptNet.Models;

namespace ChatGptNet;

internal class ChatGptClient : IChatGptClient
{
    private readonly HttpClient httpClient;

    public ChatGptClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<ChatGptResponse?> AskAsync(string message, string model, CancellationToken cancellationToken = default)
    {
        var request = new ChatGptRequest
        {
            Model = model,
            Messages = new[] { new ChatGptMessage
                {
                    Role=ChatGptRoles.User,
                    Content=message
                }
            }
        };

        using var httpResponse = await httpClient.PostAsJsonAsync("chat/completions", request, cancellationToken);
        if (!httpResponse.IsSuccessStatusCode)
        {
            var errorRoot = await httpResponse.Content.ReadFromJsonAsync<ChatGptApiErrorRoot>(cancellationToken: cancellationToken);
            throw new ChatGptApiException(errorRoot!.Error, httpResponse.StatusCode);
        }

        var response = await httpResponse.Content.ReadFromJsonAsync<ChatGptResponse>(cancellationToken: cancellationToken);
        return response;
    }
}
