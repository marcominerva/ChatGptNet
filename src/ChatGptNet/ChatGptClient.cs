using System.Net.Http.Json;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ChatGptNet.Exceptions;
using ChatGptNet.Models;
using ChatGptNet.Models.Common;
using ChatGptNet.Models.Embeddings;

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
        conversationId = (conversationId == Guid.Empty) ? Guid.NewGuid() : conversationId;

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

    public async Task<ChatGptResponse> AskAsync(Guid conversationId, string message, ChatGptToolParameters? toolParameters = null, ChatGptParameters? parameters = null, string? model = null, bool addToConversationHistory = true, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        // Ensures that conversationId isn't empty.
        conversationId = (conversationId == Guid.Empty) ? Guid.NewGuid() : conversationId;

        var messages = await CreateMessageListAsync(conversationId, message, cancellationToken);
        var request = CreateChatGptRequest(messages, toolParameters, false, parameters, model);

        var requestUri = options.ServiceConfiguration.GetChatCompletionEndpoint(model ?? options.DefaultModel);
        using var httpResponse = await httpClient.PostAsJsonAsync(requestUri, request, jsonSerializerOptions, cancellationToken);

        var response = await httpResponse.Content.ReadFromJsonAsync<ChatGptResponse>(jsonSerializerOptions, cancellationToken: cancellationToken);
        NormalizeResponse(httpResponse, response!, conversationId, model ?? options.DefaultModel);

        if (response!.IsSuccessful)
        {
            if (addToConversationHistory)
            {
                // Adds the response message to the conversation cache.
                await AddAssistantResponseAsync(conversationId, messages, response.Choices.First().Message, cancellationToken);
            }
        }
        else if (options.ThrowExceptionOnError)
        {
            throw new ChatGptException(response.Error, httpResponse.StatusCode);
        }

        return response;
    }

    public async IAsyncEnumerable<ChatGptResponse> AskStreamAsync(Guid conversationId, string message, ChatGptParameters? parameters = null, string? model = null, bool addToConversationHistory = true, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        // Ensures that conversationId isn't empty.
        conversationId = (conversationId == Guid.Empty) ? Guid.NewGuid() : conversationId;

        var messages = await CreateMessageListAsync(conversationId, message, cancellationToken);
        var request = CreateChatGptRequest(messages, null, true, parameters, model);

        var requestUri = options.ServiceConfiguration.GetChatCompletionEndpoint(model ?? options.DefaultModel);
        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri)
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

                IEnumerable<ChatGptPromptFilterResults>? promptFilterResults = null;

                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync() ?? string.Empty;
                    if (line.StartsWith("data: {"))
                    {
                        var json = line["data: ".Length..];

                        var response = JsonSerializer.Deserialize<ChatGptResponse>(json, jsonSerializerOptions);
                        response!.ConversationId = conversationId;

                        promptFilterResults ??= response.PromptFilterResults;
                        response.PromptFilterResults = promptFilterResults;

                        var choice = response.Choices?.FirstOrDefault();

                        if (choice?.Delta is not null)
                        {
                            choice.Delta.Role = ChatGptRoles.Assistant;
                            var content = choice.Delta.Content;

                            if (choice.FinishReason == ChatGptFinishReasons.ContentFilter)
                            {
                                // The response has been filtered by the content filtering system. Returns the response as is.
                                yield return response;
                            }
                            else if (!string.IsNullOrEmpty(content))
                            {
                                // It is a normal assistant response.
                                if (contentBuilder.Length == 0)
                                {
                                    // If this is the first response, trims all the initial special characters.
                                    content = content.TrimStart('\n');
                                    choice.Delta.Content = content;
                                }

                                // Yields the response only if there is an actual content.
                                if (content != string.Empty)
                                {
                                    contentBuilder.Append(content);
                                    yield return response;
                                }
                            }
                        }
                    }
                    else if (line.StartsWith("data: [DONE]"))
                    {
                        break;
                    }
                }
            }

            if (addToConversationHistory)
            {
                // Adds the response message to the conversation cache.
                await AddAssistantResponseAsync(conversationId, messages, new()
                {
                    Role = ChatGptRoles.Assistant,
                    Content = contentBuilder.ToString()
                }, cancellationToken);
            }
        }
        else
        {
            var response = await httpResponse.Content.ReadFromJsonAsync<ChatGptResponse>(cancellationToken: cancellationToken);
            NormalizeResponse(httpResponse, response!, conversationId, model ?? options.DefaultModel);

            if (options.ThrowExceptionOnError)
            {
                throw new ChatGptException(response!.Error, httpResponse.StatusCode);
            }

            yield return response!;
        }
    }

    public async Task<IEnumerable<ChatGptMessage>> GetConversationAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var conversationHistory = await cache.GetAsync(conversationId, cancellationToken);
        var messages = conversationHistory?.ToList() ?? Enumerable.Empty<ChatGptMessage>();

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
                // Preserves the system message.
                messages = messages.Where(m => m.Role == ChatGptRoles.System);
                await cache.SetAsync(conversationId, messages, options.MessageExpiration, cancellationToken);
            }
        }
    }

    public async Task<Guid> LoadConversationAsync(Guid conversationId, IEnumerable<ChatGptMessage> messages, bool replaceHistory = true, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(messages);

        // Ensures that conversationId isn't empty.
        conversationId = (conversationId == Guid.Empty) ? Guid.NewGuid() : conversationId;

        // If messages must replace history, just use the current list, discarding all the previously cached content.
        if (!replaceHistory)
        {
            // Otherwise, retrieves the current history and adds the messages.
            var conversationHistory = await cache.GetAsync(conversationId, cancellationToken) ?? Enumerable.Empty<ChatGptMessage>();
            messages = conversationHistory.Union(messages);
        }

        // If messages.Count() > ChatGptOptions.MessageLimit, the UpdateCacheAsync method takes care of taking only the last messages.
        await UpdateCacheAsync(conversationId, messages.ToList(), cancellationToken);

        return conversationId;
    }

    public async Task AddInteractionAsync(Guid conversationId, string question, string answer, CancellationToken cancellationToken = default)
    {
        ThrowIfEmptyConversationId(conversationId, nameof(conversationId));
        ArgumentNullException.ThrowIfNull(question);
        ArgumentNullException.ThrowIfNull(answer);

        var messages = await cache.GetAsync(conversationId, cancellationToken) ?? Enumerable.Empty<ChatGptMessage>();
        messages = messages.Union([
            new()
            {
                Role = ChatGptRoles.User,
                Content = question
            },
            new()
            {
                Role = ChatGptRoles.Assistant,
                Content = answer
            }
        ]);

        await UpdateCacheAsync(conversationId, messages, cancellationToken);
    }

    public async Task AddToolResponseAsync(Guid conversationId, string? toolId, string name, string content, CancellationToken cancellationToken = default)
    {
        ThrowIfEmptyConversationId(conversationId, nameof(conversationId));
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(content);

        var messages = await cache.GetAsync(conversationId, cancellationToken);
        if (!messages?.Any() ?? true)
        {
            throw new InvalidOperationException("Cannot add a tool/function response message if the conversation history is empty");
        }

        messages = messages!.Append(new()
        {
            ToolCallId = toolId,
            Role = toolId is not null ? ChatGptRoles.Tool : ChatGptRoles.Function,
            Name = name,
            Content = content
        });

        await UpdateCacheAsync(conversationId, messages, cancellationToken);
    }

    public async Task<EmbeddingResponse> GenerateEmbeddingAsync(IEnumerable<string> messages, string? model = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(messages);

        var request = CreateEmbeddingRequest(messages, model);

        var requestUri = options.ServiceConfiguration.GetEmbeddingEndpoint(model ?? options.DefaultEmbeddingModel);
        using var httpResponse = await httpClient.PostAsJsonAsync(requestUri, request, jsonSerializerOptions, cancellationToken);

        var response = await httpResponse.Content.ReadFromJsonAsync<EmbeddingResponse>(jsonSerializerOptions, cancellationToken: cancellationToken);
        NormalizeResponse(httpResponse, response!, model ?? options.DefaultEmbeddingModel);

        if (!response!.IsSuccessful && options.ThrowExceptionOnError)
        {
            throw new EmbeddingException(response.Error, httpResponse.StatusCode);
        }

        return response;
    }

    private async Task<IList<ChatGptMessage>> CreateMessageListAsync(Guid conversationId, string message, CancellationToken cancellationToken = default)
    {
        // Checks whether a list of messages for the given conversationId already exists.
        var conversationHistory = await cache.GetAsync(conversationId, cancellationToken);
        var messages = conversationHistory?.ToList() ?? [];

        messages.Add(new()
        {
            Role = ChatGptRoles.User,
            Content = message
        });

        return messages;
    }

    private ChatGptRequest CreateChatGptRequest(IEnumerable<ChatGptMessage> messages, ChatGptToolParameters? toolParameters, bool stream, ChatGptParameters? parameters, string? model)
        => new()
        {
            Model = model ?? options.DefaultModel,
            Messages = messages,

            // If the tool parameters uses the new Tools and ToolChoice properties, that are available only with the latest models.
            Tools = toolParameters?.Tools,
            ToolChoice = toolParameters?.ToolChoice switch
            {
                ChatGptToolChoices.None or ChatGptToolChoices.Auto => toolParameters.ToolChoice,
                { } => JsonDocument.Parse($$"""{ "type": "{{ChatGptToolTypes.Function}}", "{{ChatGptToolTypes.Function}}": {  "name": "{{toolParameters.ToolChoice}}" } }"""),
                _ => null
            },

            // If the tool parameters uses the legacy function properties.
            Functions = toolParameters?.Functions,
            FunctionCall = toolParameters?.FunctionCall switch
            {
                ChatGptToolChoices.None or ChatGptToolChoices.Auto => toolParameters.FunctionCall,
                { } => JsonDocument.Parse($$"""{ "name": "{{toolParameters.FunctionCall}}" }"""),
                _ => null
            },

            Stream = stream,
            Seed = parameters?.Seed ?? options.DefaultParameters.Seed,
            Temperature = parameters?.Temperature ?? options.DefaultParameters.Temperature,
            TopP = parameters?.TopP ?? options.DefaultParameters.TopP,
            MaxTokens = parameters?.MaxTokens ?? options.DefaultParameters.MaxTokens,
            PresencePenalty = parameters?.PresencePenalty ?? options.DefaultParameters.PresencePenalty,
            FrequencyPenalty = parameters?.FrequencyPenalty ?? options.DefaultParameters.FrequencyPenalty,
            User = options.User,
            ResponseFormat = parameters?.ResponseFormat ?? options.DefaultParameters.ResponseFormat
        };

    private EmbeddingRequest CreateEmbeddingRequest(IEnumerable<string> messages, string? model = null)
        => new()
        {
            Model = model ?? options.DefaultEmbeddingModel,
            Input = messages
        };

    private async Task AddAssistantResponseAsync(Guid conversationId, IList<ChatGptMessage> messages, ChatGptMessage? message, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(message?.Content?.Trim()) || message?.FunctionCall is not null || (message?.ToolCalls?.Any() ?? false))
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

            messages = conversation;
        }

        await cache.SetAsync(conversationId, messages, options.MessageExpiration, cancellationToken);
    }

    private static void NormalizeResponse(HttpResponseMessage httpResponse, ChatGptResponse response, Guid conversationId, string? model)
    {
        response.ConversationId = conversationId;
        NormalizeResponse(httpResponse, response, model);
    }

    private static void NormalizeResponse(HttpResponseMessage httpResponse, Response response, string? model)
    {
        if (string.IsNullOrWhiteSpace(response.Model) && model is not null)
        {
            response.Model = model;
        }

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

    private static void ThrowIfEmptyConversationId(Guid guid, string parameterName)
    {
        if (guid == Guid.Empty)
        {
            throw new ArgumentException($"The value {guid} is invalid", parameterName);
        }
    }
}
