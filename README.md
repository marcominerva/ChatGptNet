# ChatGPT for .NET

[![Lint Code Base](https://github.com/marcominerva/ChatGptNet/actions/workflows/linter.yml/badge.svg)](https://github.com/marcominerva/ChatGptNet/actions/workflows/linter.yml)
[![CodeQL](https://github.com/marcominerva/ChatGptNet/actions/workflows/codeql.yml/badge.svg)](https://github.com/marcominerva/ChatGptNet/actions/workflows/codeql.yml)
[![NuGet](https://img.shields.io/nuget/v/ChatGptNet.svg?style=flat-square)](https://www.nuget.org/packages/ChatGptNet)
[![Nuget](https://img.shields.io/nuget/dt/ChatGptNet)](https://www.nuget.org/packages/ChatGptNet)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/marcominerva/ChatGptNet/blob/master/LICENSE)

A ChatGPT integration library for .NET, supporting both OpenAI and Azure OpenAI Service.

## Installation

The library is available on [NuGet](https://www.nuget.org/packages/ChatGptNet). Just search for *ChatGptNet* in the **Package Manager GUI** or run the following command in the **.NET CLI**:

    dotnet add package ChatGptNet

## Configuration

Register ChatGPT service at application startup:

    builder.Services.AddChatGpt(options =>
    {
        // OpenAI.
        //options.UseOpenAI(apiKey: "", organization: "");

        // Azure OpenAI Service.
        //options.UseAzure(resourceName: "", apiKey: "", authenticationType: AzureAuthenticationType.ApiKey);

        options.DefaultModel = "my-model";
        options.MessageLimit = 16;  // Default: 10
        options.MessageExpiration = TimeSpan.FromMinutes(5);    // Default: 1 hour
    });


**ChatGptNet** supports both OpenAI and Azure OpenAI Service, so it is necessary to set the correct configuration settings based on the chosen provider:

#### OpenAI (UseOpenAI)

- _ApiKey_: it is available in the [User settings](https://platform.openai.com/account/api-keys) page of the OpenAI account (required).
- _Organization_: for users who belong to multiple organizations, you can also specify which organization is used. Usage from these API requests will count against the specified organization's subscription quota (optional).

##### Azure OpenAI Service (UseAzure)

- _ResourceName_: the name of your Azure OpenAI Resource (required).
- _ApiKey_: Azure OpenAI provides two methods for authentication. You can use either API Keys or Azure Active Directory (required).
- _AuthenticationType_: it specifies if the key is an actual API Key or an [Azure Active Directory token](https://learn.microsoft.com/azure/cognitive-services/openai/how-to/managed-identity) (optional, default: "ApiKey").

### DefaultModel

ChatGPT can be used with different models for chat completion, both on OpenAI and Azure OpenAI service. With the *DefaultModel* property, you can specify the default model that will be used, unless you pass an explicit value in the **AskAsync** method.

##### OpenAI

Currently available models are: _gpt-3.5-turbo_, _gpt-4_ and _gpt-4-32k_. They have fixed names, available in the [OpenAIChatGptModels.cs file](https://github.com/marcominerva/ChatGptNet/blob/master/src/ChatGptNet/Models/ChatGptModels.cs).

> **Note**
The _gpt-4_ and _gpt-4-32k_ models are currently in a limited beta and only accessible to those who have been granted access. You can find more information in the [models documentation page](https://platform.openai.com/docs/models/gpt-4) of the [OpenAI site](https://openai.com/).

##### Azure OpenAI Service

In Azure OpenAI Service, you're required to first [deploy a model](https://learn.microsoft.com/azure/cognitive-services/openai/how-to/create-resource?pivots=web-portal#deploy-a-model) before you can make calls. When you deploy a model, you need to assign it a name, that must match the name you use with **ChatGptNet**.

> **Note**
Some models are not available in all regions. You can refer to [Model Summary table and region availability page](https://learn.microsoft.com/azure/cognitive-services/openai/concepts/models#model-summary-table-and-region-availability) to check current availabilities.

### Caching, MessageLimit and MessageExpiration

ChatGPT is aimed to support conversational scenarios: user can talk to ChatGPT without specifying the full context for every interaction. However, conversation history isn't managed by OpenAI or Azure OpenAI service, so it's up to us to retain the current state. By default, **ChatGptNet** handles this requirement using a [MemoryCache](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.memory.memorycache) that stores messages for each conversation. The behavior can be set using the following properties:

* *MessageLimit*: specifies how many messages for each conversation must be saved. When this limit is reached, oldest messages are automatically removed.
* *MessageExpiration*: specifies the time interval used to maintain messages in cache, regardless their count.

If necessary, it is possibile to provide a custom Cache by implementing the [IChatGptCache](https://github.com/marcominerva/ChatGptNet/blob/master/src/ChatGptNet/IChatGptCache.cs) interface and then calling the **WithCache** extension method:

    public class LocalMessageCache : IChatGptCache
    {
        private readonly Dictionary<Guid, List<ChatGptMessage>> localCache = new();

        public Task SetAsync(Guid conversationId, IEnumerable<ChatGptMessage> messages, TimeSpan expiration)
        {
            localCache[conversationId] = messages.ToList();
            return Task.CompletedTask;
        }

        public Task<List<ChatGptMessage>?> GetAsync(Guid conversationId)
        {
            localCache.TryGetValue(conversationId, out var messages);
            return Task.FromResult(messages);
        }

        public Task RemoveAsync(Guid conversationId)
        {
            localCache.Remove(conversationId);
            return Task.CompletedTask;
        }
    }

    // Registers the custom cache at application startup.
    builder.Services.AddChatGpt(/* ... */).WithCache<LocalMessageCache>();

We can also set ChatGPT parameters for chat completion at startup. Check the [official documentation](https://platform.openai.com/docs/api-reference/chat/create) for the list of available parameters and their meaning.

### Configuration using an external source

The configuration can be automatically read from [IConfiguration](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.iconfiguration), using for example a _ChatGPT_ section in the _appsettings.json_ file:

    "ChatGPT": {
        "Provider": "OpenAI",           // Optional. Allowed values: OpenAI (default) or Azure
        "ApiKey": "",                   // Required
        //"Organization": "",           // Optional, used only by OpenAI
        "ResourceName": "",             // Required when using Azure OpenAI Service
        "AuthenticationType": "ApiKey", // Optional, used only by Azure OpenAI Service. Allowed values : ApiKey (default) or ActiveDirectory

        "DefaultModel": "my-model",
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

And then use the corresponding overload of che **AddChatGpt** method:

    // Adds ChatGPT service using settings from IConfiguration.
    builder.Services.AddChatGpt(builder.Configuration);

### Configuring ChatGptNet dinamically

The **AddChatGpt** method has also an overload that accepts an [IServiceProvider](https://learn.microsoft.com/dotnet/api/system.iserviceprovider) as argument. It can be used, for example, if we're in a Web API and we need to support scenarios in which every user has a different API Key that can be retrieved accessing a database via Dependency Injection:

    builder.Services.AddChatGpt((services, options) =>
    {
        var accountService = services.GetRequiredService<IAccountService>();

        // Dynamically gets the API Key from the service.
        var apiKey = "..."        

        options.UseOpenAI(apiKyey);
    });

### Configuring ChatGptNet using both IConfiguration and code

In more complex scenarios, it is possible to configure **ChatGptNet** using both code and [IConfiguration](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration.iconfiguration). This can be useful if we want to set a bunch of common properties, but at the same time we need some configuration logic. For example:

    builder.Services.AddChatGpt((services, options) =>
    {
        // Configure common properties (message limit and expiration, default parameters, ecc.) using IConfiguration.
        options.UseConfiguration(builder.Configuration);

        var accountService = services.GetRequiredService<IAccountService>();

        // Dynamically gets the API Key from the service.
        var apiKey = "..."        

        options.UseOpenAI(apiKyey);
    });

## Usage

The library can be used in any .NET application built with .NET 6.0 or later. For example, we can create a Minimal API in this way:

    app.MapPost("/api/chat/ask", async (Request request, IChatGptClient chatGptClient) =>
    {
        var response = await chatGptClient.AskAsync(request.ConversationId, request.Message);
        return TypedResults.Ok(response);
    })
    .WithOpenApi();

    // ...

    public record class Request(Guid ConversationId, string Message);

If we just want to retrieve the response message, we can call the **GetMessage** method:

    var message = response.GetMessage();

### Handling a conversation

The **AskAsync** method has an overload (the one shown in the example above) that requires a *conversationId* parameter. If we pass an empty value, a random one is generated and returned.
We can pass this value in subsequent invocations of **AskAsync** so that the library automatically retrieves previous messages of the current conversation (according to *MessageLimit* and *MessageExpiration* settings) and send them to chat completion API.

### Response streaming

Chat completion API supports response streaming. When using this feature, partial message deltas will be sent, like in ChatGPT. Tokens will be sent as data-only server-sent events as they become available. **ChatGptNet** provides response streaming using the **AskStreamAsync** method:

    // Requests a streaming response.
    var responseStream = chatGptClient.AskStreamAsync(conversationId, message);

    await foreach (var response in responseStream)
    {
        Console.Write(response.GetMessage());
        await Task.Delay(80);
    }

![](https://raw.githubusercontent.com/marcominerva/ChatGptNet/master/assets/ChatGptConsoleStreaming.gif)

Response streaming works by returning an [IAsyncEnumerable](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1), so it can be used even in a Web API project:

    app.MapGet("/api/chat/stream", (Guid? conversationId, string message, IChatGptClient chatGptClient) =>
    {
        async IAsyncEnumerable<string> Stream()
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

![](https://raw.githubusercontent.com/marcominerva/ChatGptNet/master/assets/ChatGptApiStreaming.gif)

The library is 100% compatible also with Blazor WebAssembly applications:

![](https://raw.githubusercontent.com/marcominerva/ChatGptNet/master/assets/ChatGptBlazor.WasmStreaming.gif)

Check the [Samples folder](https://github.com/marcominerva/ChatGptNet/tree/master/samples) for more information about the different implementations.

## Changing the assistant's behavior

ChatGPT supports messages with the _system_ role to influence how the assistant should behave. For example, we can tell to ChatGPT something like that:

- You are an helpful assistant
- Answer like Shakespeare
- Give me only wrong answers
- Answer in rhyme

**ChatGptNet** provides this feature using the **SetupAsync** method:

    var conversationId await = chatGptClient.SetupAsync("Answer in rhyme");

If we use the same *conversationId* when calling  **AskAsync**, then the *system* message will be automatically sent along with every request, so that the assistant will know how to behave.

> **Note**
The *system* message does not count for messages limit number.

### Deleting a conversation

Conversation history is automatically deleted when expiration time (specified by *MessageExpiration* property) is reached. However, if necessary it is possible to immediately clear the history:

    await chatGptClient.DeleteConversationAsync(conversationId, preserveSetup: false);

The _preserveSetup_ argument allows to decide whether mantain also the _system_ message that has been set with the **SetupAsync** method (default: _false_).

## Contribute

The project is constantly evolving. Contributions are welcome. Feel free to file issues and pull requests on the repo and we'll address them as we can. 

> **Warning**
Remember to work on the **develop** branch, don't use the **master** branch directly. Create Pull Requests targeting **develop**.
