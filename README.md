# ChatGPT for .NET

[![Lint Code Base](https://github.com/marcominerva/ChatGptNet/actions/workflows/linter.yml/badge.svg)](https://github.com/marcominerva/ChatGptNet/actions/workflows/linter.yml)
[![CodeQL](https://github.com/marcominerva/ChatGptNet/actions/workflows/codeql.yml/badge.svg)](https://github.com/marcominerva/ChatGptNet/actions/workflows/codeql.yml)
[![NuGet](https://img.shields.io/nuget/v/ChatGptNet.svg?style=flat-square)](https://www.nuget.org/packages/ChatGptNet)
[![Nuget](https://img.shields.io/nuget/dt/ChatGptNet)](https://www.nuget.org/packages/ChatGptNet)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/marcominerva/ChatGptNet/blob/master/LICENSE)

A ChatGPT integration library for .NET, supporting both OpenAI and Azure OpenAI Service.

## Installation

The library is available on [NuGet](https://www.nuget.org/packages/ChatGptNet). Just search for *ChatGptNet* in the **Package Manager GUI** or run the following command in the **.NET CLI**:

```shell
dotnet add package ChatGptNet
```

## Configuration

Register ChatGPT service at application startup:

```csharp
builder.Services.AddChatGpt(options =>
{
    // OpenAI.
    //options.UseOpenAI(apiKey: "", organization: "");

    // Azure OpenAI Service.
    //options.UseAzure(resourceName: "", apiKey: "", authenticationType: AzureAuthenticationType.ApiKey);

    options.DefaultModel = "my-model";
    options.DefaultEmbeddingModel = "text-embedding-ada-002",
    options.MessageLimit = 16;  // Default: 10
    options.MessageExpiration = TimeSpan.FromMinutes(5);    // Default: 1 hour
});
```

**ChatGptNet** supports both OpenAI and Azure OpenAI Service, so it is necessary to set the correct configuration settings based on the chosen provider:

#### OpenAI (UseOpenAI)

- _ApiKey_: it is available in the [User settings](https://platform.openai.com/account/api-keys) page of the OpenAI account (required).
- _Organization_: for users who belong to multiple organizations, you can also specify which organization is used. Usage from these API requests will count against the specified organization's subscription quota (optional).

##### Azure OpenAI Service (UseAzure)

- _ResourceName_: the name of your Azure OpenAI Resource (required).
- _ApiKey_: Azure OpenAI provides two methods for authentication. You can use either API Keys or Azure Active Directory (required).
- _ApiVersion_: the version of the API to use (optional). Allowed values:
  - 2023-03-15-preview
  - 2023-05-15
  - 2023-06-01-preview
  - 2023-07-01-preview
  - 2023-08-01-preview (default)
- _AuthenticationType_: it specifies if the key is an actual API Key or an [Azure Active Directory token](https://learn.microsoft.com/azure/cognitive-services/openai/how-to/managed-identity) (optional, default: "ApiKey").

### DefaultModel and DefaultEmbeddingModel

ChatGPT can be used with different models for chat completion, both on OpenAI and Azure OpenAI service. With the *DefaultModel* property, you can specify the default model that will be used, unless you pass an explicit value in the **AskAsync** or **AsyStreamAsync** methods.

Even if it is not a strictly necessary for chat conversation, the library supports also the Embedding API, on both [OpenAI](https://platform.openai.com/docs/guides/embeddings/what-are-embeddings) and [Azure OpenAI](https://learn.microsoft.com/en-us/azure/ai-services/openai/reference#embeddings).  As for chat completion, embeddings can be done with different models. With the *DefaultEmbeddingModel* property, you can specify the default model that will be used, unless you pass an explicit value in the **GetEmbeddingAsync** method.

##### OpenAI

Currently available models are: _gpt-3.5-turbo_, _gpt-3.5-turbo-16k_, _gpt-4_ and _gpt-4-32k_. They have fixed names, available in the [OpenAIChatGptModels.cs file](https://github.com/marcominerva/ChatGptNet/blob/master/src/ChatGptNet/Models/OpenAIChatGptModels.cs).

##### Azure OpenAI Service

In Azure OpenAI Service, you're required to first [deploy a model](https://learn.microsoft.com/azure/cognitive-services/openai/how-to/create-resource?pivots=web-portal#deploy-a-model) before you can make calls. When you deploy a model, you need to assign it a name, that must match the name you use with **ChatGptNet**.

> **Note**
Some models are not available in all regions. You can refer to [Model Summary table and region availability page](https://learn.microsoft.com/azure/cognitive-services/openai/concepts/models#model-summary-table-and-region-availability) to check current availabilities.

### Caching, MessageLimit and MessageExpiration

ChatGPT is aimed to support conversational scenarios: user can talk to ChatGPT without specifying the full context for every interaction. However, conversation history isn't managed by OpenAI or Azure OpenAI service, so it's up to us to retain the current state. By default, **ChatGptNet** handles this requirement using a [MemoryCache](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.memory.memorycache) that stores messages for each conversation. The behavior can be set using the following properties:

* *MessageLimit*: specifies how many messages for each conversation must be saved. When this limit is reached, oldest messages are automatically removed.
* *MessageExpiration*: specifies the time interval used to maintain messages in cache, regardless their count.

If necessary, it is possibile to provide a custom Cache by implementing the [IChatGptCache](https://github.com/marcominerva/ChatGptNet/blob/master/src/ChatGptNet/IChatGptCache.cs) interface and then calling the **WithCache** extension method:

```csharp
public class LocalMessageCache : IChatGptCache
{
    private readonly Dictionary<Guid, IEnumerable<ChatGptMessage>> localCache = new();

    public Task SetAsync(Guid conversationId, IEnumerable<ChatGptMessage> messages, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        localCache[conversationId] = messages.ToList();
        return Task.CompletedTask;
    }

    public Task<IEnumerable<ChatGptMessage>?> GetAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        localCache.TryGetValue(conversationId, out var messages);
        return Task.FromResult(messages);
    }

    public Task RemoveAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        localCache.Remove(conversationId);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var exists = localCache.ContainsKey(conversationId);
        return Task.FromResult(exists);
    }
}

// Registers the custom cache at application startup.
builder.Services.AddChatGpt(/* ... */).WithCache<LocalMessageCache>();
```

We can also set ChatGPT parameters for chat completion at startup. Check the [official documentation](https://platform.openai.com/docs/api-reference/chat/create) for the list of available parameters and their meaning.

### Configuration using an external source

The configuration can be automatically read from [IConfiguration](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.iconfiguration), using for example a _ChatGPT_ section in the _appsettings.json_ file:

```yaml
"ChatGPT": {
    "Provider": "OpenAI",               // Optional. Allowed values: OpenAI (default) or Azure
    "ApiKey": "",                       // Required
    //"Organization": "",               // Optional, used only by OpenAI
    "ResourceName": "",                 // Required when using Azure OpenAI Service
    "ApiVersion": "2023-08-01-preview", // Optional, used only by Azure OpenAI Service (default: 2023-08-01-preview)
    "AuthenticationType": "ApiKey",     // Optional, used only by Azure OpenAI Service. Allowed values: ApiKey (default) or ActiveDirectory

    "DefaultModel": "my-model",
    "DefaultEmbeddingModel": "text-embedding-ada-002", // Optional, set it if you want to use embedding
    "MessageLimit": 20,
    "MessageExpiration": "00:30:00",
    "ThrowExceptionOnError": true
    //"User": "UserName",
    //"DefaultParameters": {
    //    "Temperature": 0.8,
    //    "TopP": 1,
    //    "MaxTokens": 500,
    //    "PresencePenalty": 0,
    //    "FrequencyPenalty": 0
    //}
}
```

And then use the corresponding overload of che **AddChatGpt** method:

```csharp
// Adds ChatGPT service using settings from IConfiguration.
builder.Services.AddChatGpt(builder.Configuration);
```

### Configuring ChatGptNet dinamically

The **AddChatGpt** method has also an overload that accepts an [IServiceProvider](https://learn.microsoft.com/dotnet/api/system.iserviceprovider) as argument. It can be used, for example, if we're in a Web API and we need to support scenarios in which every user has a different API Key that can be retrieved accessing a database via Dependency Injection:

```csharp
builder.Services.AddChatGpt((services, options) =>
{
    var accountService = services.GetRequiredService<IAccountService>();

    // Dynamically gets the API Key from the service.
    var apiKey = "..."        

    options.UseOpenAI(apiKyey);
});
```

### Configuring ChatGptNet using both IConfiguration and code

In more complex scenarios, it is possible to configure **ChatGptNet** using both code and [IConfiguration](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.iconfiguration). This can be useful if we want to set a bunch of common properties, but at the same time we need some configuration logic. For example:

```csharp
builder.Services.AddChatGpt((services, options) =>
{
    // Configure common properties (message limit and expiration, default parameters, ecc.) using IConfiguration.
    options.UseConfiguration(builder.Configuration);

    var accountService = services.GetRequiredService<IAccountService>();

    // Dynamically gets the API Key from the service.
    var apiKey = "..."        

    options.UseOpenAI(apiKyey);
});
```

## Usage

The library can be used in any .NET application built with .NET 6.0 or later. For example, we can create a Minimal API in this way:

```csharp
app.MapPost("/api/chat/ask", async (Request request, IChatGptClient chatGptClient) =>
{
    var response = await chatGptClient.AskAsync(request.ConversationId, request.Message);
    return TypedResults.Ok(response);
})
.WithOpenApi();

// ...

public record class Request(Guid ConversationId, string Message);
```

If we just want to retrieve the response message, we can call the **GetContent** method:

```csharp
var content = response.GetContent();
```

> **Note**
If the response has been filtered by the content filtering system, **GetContent** will return *null*. So, you should always check the `response.IsContentFiltered` property before trying to access to the actual content.

### Handling a conversation

The **AskAsync** and **AskStreamAsync** (see below) methods provides overloads that require a *conversationId* parameter. If we pass an empty value, a random one is generated and returned.
We can pass this value in subsequent invocations of **AskAsync** or **AskStreamAsync**, so that the library automatically retrieves previous messages of the current conversation (according to *MessageLimit* and *MessageExpiration* settings) and send them to chat completion API.

This is the default behavior for all the chat interactions. If you want to exlude a particular interaction from the conversation history, you can set the *addToConversationHistory* argument to *false*:

```csharp
var response = await chatGptClient.AskAsync(conversationId, message, addToConversationHistory: false);
```

In this way, the message will be sent to the chat completion API, but it and the corresponding answer from ChatGPT will not be added to the conversation history.

On the other hand, in some scenarios, it could be useful to manually add a chat interaction (i.e., a question followed by an answer) to the conversation history. For example, we may want to add a message that was generated by a bot. In this case, we can use the **AddInteractionAsync** method:

```csharp
await chatGptClient.AskInteractionAsync(conversationId, question: "What is the weather like in Taggia?",
    answer: "It's Always Sunny in Taggia");
```

The question will be added as *user* message and the answer will be added as *assistant* message in the conversation history. As always, these new messages (respecting the *MessageLimit* option) will be used in subsequent invocations of **AskAsync** or **AskStreamAsync**.

### Response streaming

Chat completion API supports response streaming. When using this feature, partial message deltas will be sent, like in ChatGPT. Tokens will be sent as data-only server-sent events as they become available. **ChatGptNet** provides response streaming using the **AskStreamAsync** method:

```csharp
// Requests a streaming response.
var responseStream = chatGptClient.AskStreamAsync(conversationId, message);

await foreach (var response in responseStream)
{
    Console.Write(response.GetContent());
    await Task.Delay(80);
}
```

![](https://raw.githubusercontent.com/marcominerva/ChatGptNet/master/assets/ChatGptConsoleStreaming.gif)

> **Note**
If the response has been filtered by the content filtering system, the **GetContent** method in the _foreach_ will return *null* strings. So, you should always check the `response.IsContentFiltered` property before trying to access to the actual content.

Response streaming works by returning an [IAsyncEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1), so it can be used even in a Web API project:

```csharp
app.MapGet("/api/chat/stream", (Guid? conversationId, string message, IChatGptClient chatGptClient) =>
{
    async IAsyncEnumerable<string?> Stream()
    {
        // Requests a streaming response.
        var responseStream = chatGptClient.AskStreamAsync(conversationId.GetValueOrDefault(), message);

        // Uses the "AsDeltas" extension method to retrieve partial message deltas only.
        await foreach (var delta in responseStream.AsDeltas())
        {
            yield return delta;
            await Task.Delay(50);
        }
    }

    return Stream();
})
.WithOpenApi();
```

![](https://raw.githubusercontent.com/marcominerva/ChatGptNet/master/assets/ChatGptApiStreaming.gif)

> **Note**
If the response has been filtered by the content filtering system, the **AsDeltas** method in the _foreach_ will return *nulls* string.


The library is 100% compatible also with Blazor WebAssembly applications:

![](https://raw.githubusercontent.com/marcominerva/ChatGptNet/master/assets/ChatGptBlazor.WasmStreaming.gif)

Check out the [Samples folder](https://github.com/marcominerva/ChatGptNet/tree/master/samples) for more information about the different implementations.

## Changing the assistant's behavior

ChatGPT supports messages with the _system_ role to influence how the assistant should behave. For example, we can tell to ChatGPT something like that:

- You are an helpful assistant
- Answer like Shakespeare
- Give me only wrong answers
- Answer in rhyme

**ChatGptNet** provides this feature using the **SetupAsync** method:

```csharp
var conversationId await = chatGptClient.SetupAsync("Answer in rhyme");
```

If we use the same *conversationId* when calling  **AskAsync**, then the *system* message will be automatically sent along with every request, so that the assistant will know how to behave.

> **Note**
The *system* message does not count for messages limit number.

### Deleting a conversation

Conversation history is automatically deleted when expiration time (specified by *MessageExpiration* property) is reached. However, if necessary it is possible to immediately clear the history:

```csharp
await chatGptClient.DeleteConversationAsync(conversationId, preserveSetup: false);
```

The _preserveSetup_ argument allows to decide whether mantain also the _system_ message that has been set with the **SetupAsync** method (default: _false_).

## Function calling

With function calling, we can describe functions and have the model intelligently choose to output a JSON object containing arguments to call those functions. This is a new way to more reliably connect GPT's capabilities with external tools and APIs.

> **Note**
Currently, on Azure OpenAI Service, function calling is supported  in the following models in API version `2023-07-01-preview`:
>- gpt-35-turbo-0613
>- gpt-35-turbo-16k-0613
>- gpt-4-0613
>- gpt-4-32k-0613

**ChatGptNet** fully supports function calling by providing an overload of the **AskAsync** method that allows to specify function definitions. If this parameter is supplied, then the model will decide when it is appropiate to use one the functions. For example:

```csharp
var functions = new List<ChatGptFunction>
{
    new()
    {
        Name = "GetCurrentWeather",
        Description = "Get the current weather",
        Parameters = JsonDocument.Parse("""                                        
        {
            "type": "object",
            "properties": {
                "location": {
                    "type": "string",
                    "description": "The city and/or the zip code"
                },
                "format": {
                    "type": "string",
                    "enum": ["celsius", "fahrenheit"],
                    "description": "The temperature unit to use. Infer this from the users location."
                }
            },
            "required": ["location", "format"]
        }
        """)
    },
    new()
    {
        Name = "GetWeatherForecast",
        Description = "Get an N-day weather forecast",
        Parameters = JsonDocument.Parse("""                                        
        {
            "type": "object",
            "properties": {
                "location": {
                    "type": "string",
                    "description": "The city and/or the zip code"
                },
                "format": {
                    "type": "string",
                    "enum": ["celsius", "fahrenheit"],
                    "description": "The temperature unit to use. Infer this from the users location."
                },
                "daysNumber": {
                    "type": "integer",
                    "description": "The number of days to forecast"
                }
            },
            "required": ["location", "format", "daysNumber"]
        }
        """)
    }
};

var functionParameters = new ChatGptFunctionParameters
{
    FunctionCall = ChatGptFunctionCalls.Auto,   // This is the default if functions are present.
    Functions = functions
};

var response = await chatGptClient.AskAsync("What is the weather like in Taggia?", functionParameters);
```

We can pass an arbitrary number of functions, each one with a name, a description and a JSON schema describing the function parameters, following the [JSON Schema references](https://json-schema.org/understanding-json-schema). Under the hood, functions are injected into the system message in a syntax the model has been trained on. This means functions count against the model's context limit and are billed as input tokens. 

The response object returned by the **AskAsync** method provides a property to check if the model has selected a function call:

```csharp
if (response.IsFunctionCall)
{
    Console.WriteLine("I have identified a function to call:");

    var functionCall = response.GetFunctionCall()!;

    Console.WriteLine(functionCall.Name);
    Console.WriteLine(functionCall.Arguments);
}
```

This code will print something like this:

    I have identified a function to call:
    GetCurrentWeather
    {
        "location": "Taggia",
        "format": "celsius"
    }

Note that the API will not actually execute any function calls. It is up to developers to execute function calls using model outputs.

After the actual execution, we need to call the **AddFunctionResponseAsync** method on the **ChatGptClient** to add the response to the conversation history, just like a standard message, so that it will be automatically used for chat completion:

```csharp
// Calls the remote function API.
var functionResponse = await GetWeatherAsync(functionCall.Arguments);
await chatGptClient.AddFunctionResponseAsync(conversationId, functionCall.Name, functionResponse);
```

Check out the [Function calling sample](https://github.com/marcominerva/ChatGptNet/blob/master/samples/ChatGptFunctionCallingConsole/Application.cs#L18) for a complete implementation of this workflow.

## Content filtering

When using Azure OpenAI Service, we automatically get content filtering for free. For details about how it works, check out the [documentation](https://learn.microsoft.com/azure/ai-services/openai/concepts/content-filter). This information is returned for all scenarios when using API version `2023-06-01-preview` or later. **ChatGptNet** fully supports this object model by providing the corresponding properties in the [ChatGptResponse](https://github.com/marcominerva/ChatGptNet/blob/master/src/ChatGptNet/Models/ChatGptResponse.cs#L57) and [ChatGptChoice](https://github.com/marcominerva/ChatGptNet/blob/master/src/ChatGptNet/Models/ChatGptChoice.cs#L26) classes.

## Embeddings

[Embeddings](https://platform.openai.com/docs/guides/embeddings) allows to transform text into a vector space. This can be useful to compare the similarity of two sentences, for example. **ChatGptNet** fully supports this feature by providing the **GetEmbeddingAsync** method:

```csharp
var response = await chatGptClient.GenerateEmbeddingAsync(message);
var embeddings = response.GetEmbedding();
```

This code will give you a float array containing all the embeddings for the specified message. The length of the array depends on the model used. For example, if we use the _text-embedding-ada-002_ model, the array will contain 1536 elements.

If you need to calculate the [cosine similarity](https://en.wikipedia.org/wiki/Cosine_similarity) between two embeddings, you can use the **EmbeddingUtility.CosineSimilarity** method.

## Documentation

The full technical documentation is available [here](https://github.com/marcominerva/ChatGptNet/tree/master/docs).

## Contribute

The project is constantly evolving. Contributions are welcome. Feel free to file issues and pull requests on the repo and we'll address them as we can. 

> **Warning**
Remember to work on the **develop** branch, don't use the **master** branch directly. Create Pull Requests targeting **develop**.
