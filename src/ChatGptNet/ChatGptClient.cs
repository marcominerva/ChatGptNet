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

    public Task<Guid> SetupAsync(Guid conversationId, string message)
    {
        ArgumentNullException.ThrowIfNull(message);

        // Ensures that conversationId isn't empty.
        if (conversationId == Guid.Empty)
        {
            conversationId = Guid.NewGuid();
        }

        var messages = new List<ChatGptMessage>
        {
            new()
            {
                Role = ChatGptRoles.System,
                Content = message
            }
        };

        cache.Set(conversationId, messages, options.MessageExpiration);

        return Task.FromResult(conversationId);
    }

    public async Task<ChatGptResponse> AskAsync(Guid conversationId, string message, string? model, CancellationToken cancellationToken = default)
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
            Model = model ?? options.DefaultModel,
            Messages = messages.ToArray()
        };

        using var httpResponse = await httpClient.PostAsJsonAsync("chat/completions", request, cancellationToken);
        var response = await httpResponse.Content.ReadFromJsonAsync<ChatGptResponse>(cancellationToken: cancellationToken);
        response!.ConversationId = conversationId;

        if (response.IsSuccessful)
        {
            // Adds the response message to the conversation cache.
            messages.Add(response.Choices[0].Message);

            // If the maximum number of messages has been reached, deletes the oldest ones.
            // Note: system message does not count for message limit.
            var conversation = messages.Where(m => m.Role != ChatGptRoles.System);
            if (conversation.Count() > options.MessageLimit)
            {
                conversation = conversation.TakeLast(options.MessageLimit);

                // If the first message was of role system, adds it back in.
                if (messages[0].Role == ChatGptRoles.System)
                {
                    conversation = conversation.Prepend(messages[0]);
                }

                messages = conversation.ToList();
            }

            cache.Set(conversationId, messages, options.MessageExpiration);
        }
        else if (options.ThrowExceptionOnError)
        {
            throw new ChatGptException(response.Error!, httpResponse.StatusCode);
        }

        return response;
    }

    public Task DeleteConversationAsync(Guid conversationId)
    {
        cache.Remove(conversationId);
        return Task.CompletedTask;
    }
}
