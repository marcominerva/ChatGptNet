using System.Net.Http.Json;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ChatGptNet.Exceptions;
using ChatGptNet.Models;

namespace ChatGptNet;

internal class ChatGptClient : IChatGptClient
{
    private readonly HttpClient httpClient;
    private readonly IChatGptCache cache;
    private readonly ChatGptOptions options;

    private static readonly JsonSerializerOptions jsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public ChatGptClient(HttpClient httpClient, IChatGptCache cache, ChatGptOptions options)
    {
        this.httpClient = httpClient;

        foreach (var header in options.ServiceConfiguration.GetRequestHeaders())
        {
            this.httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
        }

        this.cache = cache;
        this.options = options;
    }

    public async Task<Guid> SetupAsync(Guid conversationId, string message, CancellationToken cancellationToken = default)
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

        await cache.SetAsync(conversationId, messages, options.MessageExpiration, cancellationToken);
        return conversationId;
    }

    public async Task<ChatGptResponse> AskAsync(Guid conversationId, string message, ChatGptFunctionParameters? functionParameters = null, ChatGptParameters? parameters = null, string? model = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        // Ensures that conversationId isn't empty.
        if (conversationId == Guid.Empty)
        {
            conversationId = Guid.NewGuid();
        }

        var messages = await CreateMessageListAsync(conversationId, message, cancellationToken);
        var request = CreateRequest(messages, functionParameters, false, parameters, model);

        var requestUri = options.ServiceConfiguration.GetServiceEndpoint(model ?? options.DefaultModel);
        using var httpResponse = await httpClient.PostAsJsonAsync(requestUri, request, jsonSerializerOptions, cancellationToken);

        var response = await httpResponse.Content.ReadFromJsonAsync<ChatGptResponse>(jsonSerializerOptions, cancellationToken: cancellationToken);
        NormalizeResponse(httpResponse, response!, conversationId);

        if (response!.IsSuccessful)
        {
            // Adds the response message to the conversation cache.
            await AddAssistantResponseAsync(conversationId, messages, response.Choices.First().Message, cancellationToken);
        }
        else if (options.ThrowExceptionOnError)
        {
            throw new ChatGptException(response.Error, httpResponse.StatusCode);
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

        var messages = await CreateMessageListAsync(conversationId, message, cancellationToken);
        var request = CreateRequest(messages, null, true, parameters, model);

        var requestUri = options.ServiceConfiguration.GetServiceEndpoint(model ?? options.DefaultModel);
        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = new StringContent(JsonSerializer.Serialize(request, jsonSerializerOptions), Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        using var httpResponse = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (httpResponse.IsSuccessStatusCode)
        {
            var contentBuilder = new StringBuilder();

            ChatGptUsage? usage = null;
            IEnumerable<ChatGptPromptAnnotations>? promptAnnotations = null;

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

                        // Saves partial response fields that need to be added in the next response.
                        usage ??= response!.Usage;
                        promptAnnotations ??= response!.PromptAnnotations;

                        var content = response!.Choices?.FirstOrDefault()?.Delta?.Content;

                        if (!string.IsNullOrEmpty(content))
                        {
                            if (contentBuilder.Length == 0)
                            {
                                // If this is the first response, trims all the initial special characters.
                                content = content.TrimStart('\n');
                                response.Choices!.First().Delta!.Content = content;
                            }

                            // Yields the response only if there is an actual content.
                            if (content != string.Empty)
                            {
                                contentBuilder.Append(content);

                                response.ConversationId = conversationId;
                                response.Usage = usage;
                                response.PromptAnnotations = promptAnnotations;

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
            await AddAssistantResponseAsync(conversationId, messages, new()
            {
                Role = ChatGptRoles.Assistant,
                Content = contentBuilder.ToString()
            }, cancellationToken);
        }
        else
        {
            var response = await httpResponse.Content.ReadFromJsonAsync<ChatGptResponse>(cancellationToken: cancellationToken);
            NormalizeResponse(httpResponse, response!, conversationId);

            if (options.ThrowExceptionOnError)
            {
                throw new ChatGptException(response!.Error, httpResponse.StatusCode);
            }

            yield return response!;
        }
    }

    public async Task<IEnumerable<ChatGptMessage>> GetConversationAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var messages = await cache.GetAsync(conversationId, cancellationToken) ?? Enumerable.Empty<ChatGptMessage>();
        return messages;
    }

    public async Task<bool> ConversationExistsAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var exists = await cache.ExistsAsync(conversationId, cancellationToken);
        return exists;
    }

    public async Task DeleteConversationAsync(Guid conversationId, bool preserveSetup = false, CancellationToken cancellationToken = default)
    {
        if (!preserveSetup)
        {
            // We don't want to preserve setup message, so just deletes all the cache history.
            await cache.RemoveAsync(conversationId, cancellationToken);
        }
        else
        {
            var messages = await cache.GetAsync(conversationId, cancellationToken);
            if (messages is not null)
            {
                // Removes all the messages, except system ones.
                messages.RemoveAll(m => m.Role != ChatGptRoles.System);
                await cache.SetAsync(conversationId, messages, options.MessageExpiration, cancellationToken);
            }
        }
    }

    public async Task<Guid> LoadConversationAsync(Guid conversationId, IEnumerable<ChatGptMessage> messages, bool replaceHistory = true, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(messages);

        // Ensures that conversationId isn't empty.
        if (conversationId == Guid.Empty)
        {
            conversationId = Guid.NewGuid();
        }

        if (replaceHistory)
        {
            // If messages must replace history, just use the current list, discarding all the previously cached content.
            // If messages.Count() > ChatGptOptions.MessageLimit, the UpdateCache take care of taking only the last messages.
            await UpdateCacheAsync(conversationId, messages, cancellationToken);
        }
        else
        {
            // Retrieves the current history and adds new messages.
            var conversationHistory = await cache.GetAsync(conversationId, cancellationToken) ?? new List<ChatGptMessage>();
            conversationHistory.AddRange(messages);

            // If messages total length > ChatGptOptions.MessageLimit, the UpdateCache take care of taking only the last messages.
            await UpdateCacheAsync(conversationId, conversationHistory, cancellationToken);
        }

        return conversationId;
    }

    public async Task AddFunctionResponseAsync(Guid conversationId, string functionName, string content, CancellationToken cancellationToken = default)
    {
        var conversationHistory = await cache.GetAsync(conversationId, cancellationToken);
        if (!conversationHistory?.Any() ?? true)
        {
            throw new InvalidOperationException("Cannot add a function response message if the conversation history is empty");
        }

        var messages = new List<ChatGptMessage>(conversationHistory!)
        {
            new()
            {
                Role = ChatGptRoles.Function,
                Name = functionName,
                Content = content
            }
        };

        await UpdateCacheAsync(conversationId, messages, cancellationToken);
    }

    private async Task<List<ChatGptMessage>> CreateMessageListAsync(Guid conversationId, string message, CancellationToken cancellationToken = default)
    {
        // Checks whether a list of messages for the given conversationId already exists.
        var conversationHistory = await cache.GetAsync(conversationId, cancellationToken);
        List<ChatGptMessage> messages = conversationHistory is not null ? new(conversationHistory) : new();

        messages.Add(new()
        {
            Role = ChatGptRoles.User,
            Content = message
        });

        return messages;
    }

    private ChatGptRequest CreateRequest(IEnumerable<ChatGptMessage> messages, ChatGptFunctionParameters? functionParameters, bool stream, ChatGptParameters? parameters = null, string? model = null)
        => new()
        {
            Model = model ?? options.DefaultModel,
            Messages = messages,
            Functions = functionParameters?.Functions,
            FunctionCall = functionParameters?.FunctionCall switch
            {
                ChatGptFunctionCalls.None or ChatGptFunctionCalls.Auto => functionParameters.FunctionCall,
                { } => JsonDocument.Parse($$"""{ "name": "{{functionParameters.FunctionCall}}" }"""),
                _ => null
            },
            Stream = stream,
            Temperature = parameters?.Temperature ?? options.DefaultParameters.Temperature,
            TopP = parameters?.TopP ?? options.DefaultParameters.TopP,
            MaxTokens = parameters?.MaxTokens ?? options.DefaultParameters.MaxTokens,
            PresencePenalty = parameters?.PresencePenalty ?? options.DefaultParameters.PresencePenalty,
            FrequencyPenalty = parameters?.FrequencyPenalty ?? options.DefaultParameters.FrequencyPenalty,
            User = options.User,
        };

    private async Task AddAssistantResponseAsync(Guid conversationId, IList<ChatGptMessage> messages, ChatGptMessage message, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(message.Content?.Trim()))
        {
            // Adds the message to the cache only if it has a content.
            messages.Add(message);
        }

        await UpdateCacheAsync(conversationId, messages, cancellationToken);
    }

    private async Task UpdateCacheAsync(Guid conversationId, IEnumerable<ChatGptMessage> messages, CancellationToken cancellationToken = default)
    {
        // If the maximum number of messages has been reached, deletes the oldest ones.
        // Note: system message does not count for message limit.
        var conversation = messages.Where(m => m.Role != ChatGptRoles.System);

        if (conversation.Count() > options.MessageLimit)
        {
            conversation = conversation.TakeLast(options.MessageLimit);

            // If the first message was of role system, adds it back in.
            var firstMessage = messages.First();
            if (firstMessage.Role == ChatGptRoles.System)
            {
                conversation = conversation.Prepend(firstMessage);
            }

            messages = conversation.ToList();
        }

        await cache.SetAsync(conversationId, messages, options.MessageExpiration, cancellationToken);
    }

    private static void NormalizeResponse(HttpResponseMessage httpResponse, ChatGptResponse response, Guid conversationId)
    {
        response.ConversationId = conversationId;

        if (!httpResponse.IsSuccessStatusCode && response.Error is null)
        {
            response.Error = new ChatGptError
            {
                Message = httpResponse.ReasonPhrase ?? httpResponse.StatusCode.ToString(),
                Code = ((int)httpResponse.StatusCode).ToString()
            };
        }

        if (response.Error is not null)
        {
            response.Error.StatusCode = (int)httpResponse.StatusCode;
        }
    }
}
