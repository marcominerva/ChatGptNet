using System.Net.Http.Json;
using ChatGptNet.Models;

namespace ChatGptNet;

internal class ChatGptClient : IChatGptClient
{
    private readonly HttpClient httpClient;

    public ChatGptClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<Response?> AskAsync(string message, CancellationToken cancellationToken = default)
    {
        var request = new Request
        {
            ModelId = "gpt-3.5-turbo",
            Messages = new[] { new Message
            {
                Role="user",
                Content=message
            }
            }
        };

        using var httpResponse = await httpClient.PostAsJsonAsync("chat/completitions", request, cancellationToken);

        var response = await httpResponse.Content.ReadFromJsonAsync<Response>();

    }
}
