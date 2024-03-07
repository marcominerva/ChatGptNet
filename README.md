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
    options.DefaultEmbeddingModel = "text-embedding-ada-002";
    options.MessageLimit = 16;  // Default: 10
    options.MessageExpiration = TimeSpan.FromMinutes(5);    // Default: 1 hour
    options.DefaultParameters = new ChatGptParameters
    {
        MaxTokens = 800,
        Temperature = 0.7
    };
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
  - 2023-05-15
  - 2023-06-01-preview
  - 2024-02-15-preview (default)
- _AuthenticationType_: it specifies if the key is an actual API Key or an [Azure Active Directory token](https://learn.microsoft.com/azure/cognitive-services/openai/how-to/managed-identity) (optional, default: "ApiKey").

### DefaultModel and DefaultEmbeddingModel

ChatGPT can be used with different models for chat completion, both on OpenAI and Azure OpenAI service. With the *DefaultModel* property, you can specify the default model that will be used, unless you pass an explicit value in the **AskAsync** or **AsyStreamAsync** methods.

Even if it is not a strictly necessary for chat conversation, the library supports also the Embedding API, on both [OpenAI](https://platform.openai.com/docs/guides/embeddings/what-are-embeddings) and [Azure OpenAI](https://learn.microsoft.com/en-us/azure/ai-services/openai/reference#embeddings).  As for chat completion, embeddings can be done with different models. With the *DefaultEmbeddingModel* property, you can specify the default model that will be used, unless you pass an explicit value in the **GetEmbeddingAsync** method.

##### OpenAI

Currently available models are:
- gpt-3.5-turbo,
- gpt-3.5-turbo-16k,
- gpt-4,
- gpt-4-32k
- gpt-4-turbo-preview
- gpt-4-vision-preview

They have fixed names, available in the [OpenAIChatGptModels.cs file](https://github.com/marcominerva/ChatGptNet/blob/master/src/ChatGptNet/Models/OpenAIChatGptModels.cs).

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

```
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
    "ThrowExceptionOnError": true       // Optional, default: true
    //"User": "UserName",
    //"DefaultParameters": {
    //    "Temperature": 0.8,
    //    "TopP": 1,
    //    "MaxTokens": 500,
    //    "PresencePenalty": 0,
    //    "FrequencyPenalty": 0,
    //    "ResponseFormat": { "Type": "text" }, // Allowed values for Type: text (default) or json_object
    //    "Seed": 42                            // Optional (any integer value)
    //},
    //"DefaultEmbeddingParameters": {
    //    "Dimensions": 1536
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

### Configuring HTTP Client

**ChatGptNet** uses an [HttpClient](https://docs.microsoft.com/dotnet/api/system.net.http.httpclient) to call the chat completion and embedding APIs. If you need to customize it, you can use the overload of the **AddChatGpt** method that accepts an [Action&lt;IHttpClientBuiler&gt;](https://learn.microsoft.com/dotnet/api/microsoft.extensions.dependencyinjection.ihttpclientbuilder) as argument. For example, if you want to add resiliency to the HTTP client (let's say a retry policy), you can use [Polly](https://github.com/App-vNext/Polly):

```csharp
// using Microsoft.Extensions.DependencyInjection;
// Requires: Microsoft.Extensions.Http.Resilience

builder.Services.AddChatGpt(context.Configuration,
    httpClient =>
    {
        // Configures retry policy on the inner HttpClient using Polly.
        httpClient.AddStandardResilienceHandler(options =>
        {
            options.AttemptTimeout.Timeout = TimeSpan.FromMinutes(1);
            options.CircuitBreaker.SamplingDuration = TimeSpan.FromMinutes(3);
            options.TotalRequestTimeout.Timeout = TimeSpan.FromMinutes(3);
        });
    })
```

More information about this topic is available on the [official documentation](https://learn.microsoft.com/dotnet/core/resilience/http-resilience).

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

#### Using parameters

Using configuration, it is possible to set default parameters for chat completion. However, we can also specify parameters for each request, using the **AskAsync** or **AskStreamAsync** overloads that accepts a [ChatGptParameters](https://github.com/marcominerva/ChatGptNet/blob/master/src/ChatGptNet/Models/ChatGptParameters.cs) object:

```csharp
var response = await chatGptClient.AskAsync(conversationId, message, new ChatGptParameters
{
    MaxTokens = 150,
    Temperature = 0.7
});
```

We don't need to specify all the parameters, only the ones we want to override. The other ones will be taken from the default configuration.

##### Seed and system fingerprint

ChatGPT is known to be non deterministic. This means that the same input can produce different outputs. To try to control this behavior, we can use the _Temperature_ and _TopP_ parameters. For example, setting the _Temperature_ to values near to 0 makes the model more deterministic, while setting it to values near to 1 makes the model more creative.
However, this is not always enough to get the same output for the same input. To address this issue, OpenAI introduced the **Seed** parameter. If specified, the model should sample deterministically, such that repeated requests with the same seed and parameters should return the same result. Nevertheless, determinism is not guaranteed neither in this case, and you should refer to the _SystemFingerprint_ response parameter to monitor changes in the backend. Changes in this values mean that the backend configuration has changed, and this might impact determinism.

As always, the _Seed_ property can be specified in the default configuration or in the **AskAsync** or **AskStreamAsync** overloads that accepts a [ChatGptParameters](https://github.com/marcominerva/ChatGptNet/blob/master/src/ChatGptNet/Models/ChatGptParameters.cs).

> **Note**
_Seed_ and _SystemFingerprint_ are only supported by the most recent models, such as _gpt-4-1106-preview_.

##### Response format

If you want to forse the response in JSON format, you can use the _ResponseFormat_ parameter:

```csharp
var response = await chatGptClient.AskAsync(conversationId, message, new ChatGptParameters
{
    ResponseFormat = ChatGptResponseFormat.Json,
});
```

In this way, the response will always be a valid JSON. Note that must also instruct the model to produce JSON via a system or user message. If you don't do this, the model will return an error.


As always, the _ResponseFormat_ property can be specified in the default configuration or in the **AskAsync** or **AskStreamAsync** overloads that accepts a [ChatGptParameters](https://github.com/marcominerva/ChatGptNet/blob/master/src/ChatGptNet/Models/ChatGptParameters.cs).

> **Note**
_ResponseFormat_ is only supported by the most recent models, such as _gpt-4-1106-preview_.

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
await chatGptClient.AddInteractionAsync(conversationId, question: "What is the weather like in Taggia?",
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

## Tool and Function calling

With function calling, we can describe functions and have the model intelligently choose to output a JSON object containing arguments to call those functions. This is a new way to more reliably connect GPT's capabilities with external tools and APIs.

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
                    "description": "The temperature unit to use. Infer this from the user's location."
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
                    "description": "The temperature unit to use. Infer this from the user's location."
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

var toolParameters = new ChatGptToolParameters
{
    FunctionCall = ChatGptToolChoices.Auto,   // This is the default if functions are present.
    Functions = functions
};

var response = await chatGptClient.AskAsync("What is the weather like in Taggia?", toolParameters);
```

We can pass an arbitrary number of functions, each one with a name, a description and a JSON schema describing the function parameters, following the [JSON Schema references](https://json-schema.org/understanding-json-schema). Under the hood, functions are injected into the system message in a syntax the model has been trained on. This means functions count against the model's context limit and are billed as input tokens. 

The response object returned by the **AskAsync** method provides a property to check if the model has selected a function call:

```csharp
if (response.ContainsFunctionCalls())
{
    Console.WriteLine("I have identified a function to call:");

    var functionCall = response.GetFunctionCall()!;

    Console.WriteLine(functionCall.Name);
    Console.WriteLine(functionCall.Arguments);
}
```

This code will print something like this:

```
I have identified a function to call:
GetCurrentWeather
{
    "location": "Taggia",
    "format": "celsius"
}
```

Note that the API will not actually execute any function calls. It is up to developers to execute function calls using model outputs.

After the actual execution, we need to call the **AddToolResponseAsync** method on the **ChatGptClient** to add the response to the conversation history, just like a standard message, so that it will be automatically used for chat completion:

```csharp
// Calls the remote function API.
var functionResponse = await GetWeatherAsync(functionCall.Arguments);
await chatGptClient.AddToolResponseAsync(conversationId, functionCall, functionResponse);
```

Newer models like _gpt-4-1106-preview_ support a more general approach to functions, the **Tool calling**. When you send a request, you can specify a list of tools the model may call. Currently, only functions are supported, but in future release other types of tools will be available.

To use Tool calling instead of direct Function calling, you need to set the _ToolChoice_ and _Tools_ properties in the **ChatGptToolParameters** object (instead of _FunctionCall_ and _Function_, as in previous example):

```csharp
var toolParameters = new ChatGptToolParameters
{
    ToolChoice = ChatGptToolChoices.Auto,   // This is the default if functions are present.
    Tools = functions.ToTools()
};
```

The **ToTools** extension method is used to convert a list of [ChatGptFunction](https://github.com/marcominerva/ChatGptNet/blob/master/src/ChatGptNet/Models/ChatGptFunction.cs) to a list of tools.

If you use this new approach, of course you still need to check if the model has selected a tool call, using the same approach shown before.
Then, after the actual execution of the function, you have to call the **AddToolResponseAsync** method, but in this case you need to specify the tool (not the function) to which the response refers:

```csharp
var tool = response.GetToolCalls()!.First();
var functionCall = response.GetFunctionCall()!;

// Calls the remote function API.
var functionResponse = await GetWeatherAsync(functionCall.Arguments);

await chatGptClient.AddToolResponseAsync(conversationId, tool, functionResponse);
```

Finally, you need to resend the original message to the chat completion API, so that the model can continue the conversation taking into account the function call response. Check out the [Function calling sample](https://github.com/marcominerva/ChatGptNet/blob/master/samples/ChatGptFunctionCallingConsole/Application.cs#L18) for a complete implementation of this workflow.

## Content filtering

When using Azure OpenAI Service, we automatically get content filtering for free. For details about how it works, check out the [documentation](https://learn.microsoft.com/azure/ai-services/openai/concepts/content-filter). This information is returned for all scenarios when using API version `2023-06-01-preview` or later. **ChatGptNet** fully supports this object model by providing the corresponding properties in the [ChatGptResponse](https://github.com/marcominerva/ChatGptNet/blob/master/src/ChatGptNet/Models/ChatGptResponse.cs#L57) and [ChatGptChoice](https://github.com/marcominerva/ChatGptNet/blob/master/src/ChatGptNet/Models/ChatGptChoice.cs#L26) classes.

## Embeddings

[Embeddings](https://platform.openai.com/docs/guides/embeddings) allows to transform text into a vector space. This can be useful to compare the similarity of two sentences, for example. **ChatGptNet** fully supports this feature by providing the **GetEmbeddingAsync** method:

```csharp
var response = await chatGptClient.GenerateEmbeddingAsync(message);
var embeddings = response.GetEmbedding();
```

This code will give you a float array containing all the embeddings for the specified message. The length of the array depends on the model used:

| Model| Output dimension |
| - | - |
| text-embedding-ada-002 | 1536 |
| text-embedding-3-small | 1536 |
| text-embedding-3-large | 3072 |

Newer models like _text-embedding-3-small_ and _text-embedding-3-large_ allows developers to trade-off performance and cost of using embeddings. Specifically, developers can shorten embeddings without the embedding losing its concept-representing properties.

As for ChatGPT, this settings can be done in various ways:

- Via code:

```csharp
builder.Services.AddChatGpt(options =>
{
    // ...

    options.DefaultEmbeddingParameters = new EmbeddingParameters
    {
        Dimensions = 256
    };
});
```

- Using the _appsettings.json_ file:

```
"ChatGPT": {    
    "DefaultEmbeddingParameters": {
        "Dimensions": 256
    }
}
```

Then, if you want to change the dimension for a particular request, you can specify the *EmbeddingParameters* argument in the **GetEmbeddingAsync** invocation:

```csharp
var response = await chatGptClient.GenerateEmbeddingAsync(request.Message, new EmbeddingParameters
{
    Dimensions = 512
});

var embeddings = response.GetEmbedding();   // The length of the array is 512
```

If you need to calculate the [cosine similarity](https://en.wikipedia.org/wiki/Cosine_similarity) between two embeddings, you can use the **EmbeddingUtility.CosineSimilarity** method.

## Documentation

The full technical documentation is available [here](https://github.com/marcominerva/ChatGptNet/tree/master/docs).

## Contribute

The project is constantly evolving. Contributions are welcome. Feel free to file issues and pull requests on the repo and we'll address them as we can. 

> **Warning**
Remember to work on the **develop** branch, don't use the **master** branch directly. Create Pull Requests targeting **develop**.
