using System.Net.Http.Json;
using ChatGptNet.Exceptions;
using ChatGptNet.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ChatGptNet;

internal class ChatGptClient : IChatGptClient
{
    private readonly HttpClient httpClient;
    private readonly IMemoryCache cache;
    private readonly ChatGptOptions options;

    public ChatGptClient(HttpClient httpClient, IMemoryCache cache, ChatGptOptions options)
    {
        this.httpClient = httpClient;
        this.cache = cache;
        this.options = options;
    }

    public async Task<ChatGptResponse> AskAsync(Guid conversationId, string message, string model, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        // Ensures that conversationId isn't empty.
        if (conversationId == Guid.Empty)
        {
            conversationId = Guid.NewGuid();
        }

        // Checks whether a list of messages for the given conversationId already exists.
        if (!cache.TryGetValue<IList<ChatGptMessage>>(conversationId, out var messages))
        {
            messages = new List<ChatGptMessage>();
        }

        messages!.Add(new()
        {
            Role = ChatGptRoles.User,
            Content = message
        });

        var request = new ChatGptRequest
        {
            Model = model,
            Messages = messages.ToArray()
        };

        using var httpResponse = await httpClient.PostAsJsonAsync("chat/completions", request, cancellationToken);
        var response = await httpResponse.Content.ReadFromJsonAsync<ChatGptResponse>(cancellationToken: cancellationToken);
        response!.ConversationId = conversationId;

        if (!httpResponse.IsSuccessStatusCode && options.ThrowExceptionOnError)
        {
            throw new ChatGptException(response.Error!, httpResponse.StatusCode);
        }

        // Adds the response message to the conversation cache.
        if (response.Choices?.Any() ?? false)
        {
            messages.Add(response.Choices[0].Message);
        }

        // If the maximum number of messages has been reached, deletes the oldest ones.
        if (messages.Count > options.MessageLimit)
        {
            messages = messages.TakeLast(options.MessageLimit).ToList();
        }

        cache.Set(conversationId, messages, options.MessageExpiration);

        return response;
    }

    public Task DeleteConversationAsync(Guid conversationId)
    {
        if (cache.TryGetValue(conversationId, out var _))
        {
            cache.Remove(conversationId);
        }

        return Task.CompletedTask;
    }
}
