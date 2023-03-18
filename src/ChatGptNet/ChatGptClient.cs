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

    public Task<Guid> SetupAsync(string message)
    {
        ArgumentNullException.ThrowIfNull(message);

        var conversationId = Guid.NewGuid();
        SetupAsync(conversationId, message, ChatGptModels.Gpt35Turbo);

        return Task.FromResult(conversationId);
    }

    public Task SetupAsync(Guid conversationId, string message, string model)
    {
        ArgumentNullException.ThrowIfNull(message);

        var messages = new List<ChatGptMessage>
        {
            new()
            {
                Role = ChatGptRoles.System,
                Content = message
            }
        };

        cache.Set(conversationId, messages, options.MessageExpiration);

        return Task.CompletedTask;
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
        if (CountMessages(messages) > options.MessageLimit)
        {
            var newMessages = messages.TakeLast(options.MessageLimit);

            // If the first message was of role system, add it back in
            if (messages[0].Role == ChatGptRoles.System)
                newMessages = newMessages.Prepend(messages[0]);

            messages = newMessages.ToList();
        }

        cache.Set(conversationId, messages, options.MessageExpiration);

        return response;
    }

    public Task DeleteConversationAsync(Guid conversationId)
    {
        cache.Remove(conversationId);
        return Task.CompletedTask;
    }

    // Helper method used to count messages in list,
    // if the first message is of role System it shouldn't be counted
    private int CountMessages(IList<ChatGptMessage> list)
    {
        return list[0].Role == ChatGptRoles.System ? list.Count - 1 : list.Count;
    }

}
