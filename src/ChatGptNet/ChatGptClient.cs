using System.Net.Http.Json;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ChatGptNet.Exceptions;
using ChatGptNet.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ChatGptNet;

internal class ChatGptClient : IChatGptClient
{
    private readonly HttpClient httpClient;
    private readonly IMemoryCache cache;
    private readonly ChatGptOptions options;

    private static readonly JsonSerializerOptions jsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public ChatGptClient(HttpClient httpClient, IMemoryCache cache, ChatGptOptions options)
    {
        this.httpClient = httpClient;
        this.httpClient.DefaultRequestHeaders.Authorization = new("Bearer", options.ApiKey);

        if (!string.IsNullOrWhiteSpace(options.Organization))
        {
            this.httpClient.DefaultRequestHeaders.Add("OpenAI-Organization", options.Organization);
        }

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

    public async Task<ChatGptResponse> AskAsync(Guid conversationId, string message, ChatGptParameters? parameters = null, string? model = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        // Ensures that conversationId isn't empty.
        if (conversationId == Guid.Empty)
        {
            conversationId = Guid.NewGuid();
        }

        var messages = CreateMessageList(conversationId, message);
        var request = CreateRequest(messages, false, parameters, model);

        using var httpResponse = await httpClient.PostAsJsonAsync("chat/completions", request, jsonSerializerOptions, cancellationToken);
        var response = await httpResponse.Content.ReadFromJsonAsync<ChatGptResponse>(jsonSerializerOptions, cancellationToken: cancellationToken);
        response!.ConversationId = conversationId;

        if (response.IsSuccessful)
        {
            // Adds the response message to the conversation cache.
            UpdateHistory(conversationId, messages, response.Choices[0].Message);
        }
        else if (options.ThrowExceptionOnError)
        {
            throw new ChatGptException(response.Error!, httpResponse.StatusCode);
        }

        return response;
    }

    public async IAsyncEnumerable<ChatGptResponse> AskStreamAsync(Guid conversationId, string message, ChatGptParameters? parameters = null, string? model = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        // Ensures that conversationId isn't empty.
        if (conversationId == Guid.Empty)
        {
            conversationId = Guid.NewGuid();
        }

        var messages = CreateMessageList(conversationId, message);
        var request = CreateRequest(messages, true, parameters, model);

        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "chat/completions")
        {
            Content = new StringContent(JsonSerializer.Serialize(request, jsonSerializerOptions), Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        using var httpResponse = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (httpResponse.IsSuccessStatusCode)
        {
            var contentBuilder = new StringBuilder();

            using (var responseStream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken))
            {
                using var reader = new StreamReader(responseStream);

                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync() ?? string.Empty;
                    if (line.StartsWith("data: {"))
                    {
                        var json = line["data: ".Length..];
                        var response = JsonSerializer.Deserialize<ChatGptResponse>(json, jsonSerializerOptions);

                        var content = response!.Choices?[0].Delta?.Content;

                        if (!string.IsNullOrEmpty(content))
                        {
                            if (contentBuilder.Length == 0)
                            {
                                // If this is the first response, trims all the initial special characters.
                                content = content.TrimStart('\n');
                                response.Choices![0].Delta!.Content = content;
                            }

                            // Yields the response only if there is an actual content.
                            if (content != string.Empty)
                            {
                                contentBuilder.Append(content);

                                response.ConversationId = conversationId;
                                yield return response;
                            }
                        }
                    }
                    else if (line.StartsWith("data: [DONE]"))
                    {
                        break;
                    }
                }
            }

            // Adds the response message to the conversation cache.
            UpdateHistory(conversationId, messages, new()
            {
                Role = ChatGptRoles.Assistant,
                Content = contentBuilder.ToString()
            });
        }
        else
        {
            var response = await httpResponse.Content.ReadFromJsonAsync<ChatGptResponse>(cancellationToken: cancellationToken);

            if (options.ThrowExceptionOnError)
            {
                throw new ChatGptException(response!.Error!, httpResponse.StatusCode);
            }

            response!.ConversationId = conversationId;
            yield return response;
        }
    }

    public Task<IEnumerable<ChatGptMessage>> GetConversationAsync(Guid conversationId)
    {
        var messages = cache.Get<IEnumerable<ChatGptMessage>>(conversationId) ?? Enumerable.Empty<ChatGptMessage>();
        return Task.FromResult(messages);
    }

    public Task DeleteConversationAsync(Guid conversationId, bool preserveSetup = false)
    {
        if (!preserveSetup)
        {
            // We don't want to preserve setup message, so just deletes all the cache history.
            cache.Remove(conversationId);
        }
        else if (cache.TryGetValue<List<ChatGptMessage>>(conversationId, out var messages))
        {
            // Removes all the messages, except system ones.
            messages!.RemoveAll(m => m.Role != ChatGptRoles.System);
            cache.Set(conversationId, messages, options.MessageExpiration);
        }

        return Task.CompletedTask;
    }

    public Task<Guid> LoadConversationAsync(Guid conversationId, IEnumerable<ChatGptMessage> messages,
        bool replaceHistory = true)
    {
        // Ensures that messages aren't null.
        ArgumentNullException.ThrowIfNull(messages);

        // Ensures that conversationId isn't empty.
        if (conversationId == Guid.Empty)
        {
            conversationId = Guid.NewGuid();
        }

        if (replaceHistory)
        {
            LoadToCache(conversationId, messages);
        }
        else
        {
            var conversationHistory = cache.Get<IList<ChatGptMessage>>(conversationId);

            List<ChatGptMessage> history = conversationHistory is not null ? new(conversationHistory) : new();

            history.AddRange(messages);

            LoadToCache(conversationId, history);
        }

        return Task.FromResult(conversationId);
    }

    private IList<ChatGptMessage> CreateMessageList(Guid conversationId, string message)
    {
        // Checks whether a list of messages for the given conversationId already exists.
        var conversationHistory = cache.Get<IList<ChatGptMessage>>(conversationId);
        List<ChatGptMessage> messages = conversationHistory is not null ? new(conversationHistory) : new();

        messages.Add(new()
        {
            Role = ChatGptRoles.User,
            Content = message
        });

        return messages;
    }

    private ChatGptRequest CreateRequest(IList<ChatGptMessage> messages, bool stream, ChatGptParameters? parameters = null, string? model = null)
        => new()
        {
            Model = model ?? options.DefaultModel,
            Messages = messages.ToArray(),
            Stream = stream,
            Temperature = parameters?.Temperature ?? options.DefaultParameters.Temperature,
            TopP = parameters?.TopP ?? options.DefaultParameters.TopP,
            Choices = parameters?.Choices ?? options.DefaultParameters.Choices,
            MaxTokens = parameters?.MaxTokens ?? options.DefaultParameters.MaxTokens,
            PresencePenalty = parameters?.PresencePenalty ?? options.DefaultParameters.PresencePenalty,
            FrequencyPenalty = parameters?.FrequencyPenalty ?? options.DefaultParameters.FrequencyPenalty,
            User = options.User,
        };

    private void UpdateHistory(Guid conversationId, IList<ChatGptMessage> messages, ChatGptMessage message)
    {
        messages.Add(message);

        LoadToCache(conversationId, messages);
    }

    private void LoadToCache(Guid conversationId, IEnumerable<ChatGptMessage> messages)
    {
        // If the maximum number of messages has been reached, deletes the oldest ones.
        // Note: system message does not count for message limit.
        var conversation = messages.Where(m => m.Role != ChatGptRoles.System);

        if (conversation.Count() > options.MessageLimit)
        {
            conversation = conversation.TakeLast(options.MessageLimit);

            // If the first message was of role system, adds it back in.
            var gptMessage = messages.First();

            if (gptMessage.Role == ChatGptRoles.System)
            {
                conversation = conversation.Prepend(gptMessage);
            }

            messages = conversation.ToList();
        }

        cache.Set(conversationId, messages, options.MessageExpiration);
    }
}
