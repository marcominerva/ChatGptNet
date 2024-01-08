# ChatGptNet assembly

## ChatGptNet namespace

| public type | description |
| --- | --- |
| class [ChatGptBuilder](./ChatGptNet/ChatGptBuilder.md) | Represents the default builder for configuring ChatGPT. |
| class [ChatGptOptions](./ChatGptNet/ChatGptOptions.md) | Options class that provides settings for configuring ChatGPT. |
| class [ChatGptOptionsBuilder](./ChatGptNet/ChatGptOptionsBuilder.md) | Builder class to define settings for configuring ChatGPT. |
| static class [ChatGptOptionsBuilderExtensions](./ChatGptNet/ChatGptOptionsBuilderExtensions.md) | Provides extensions to configure settings for accessing ChatGPT service. |
| static class [ChatGptServiceCollectionExtensions](./ChatGptNet/ChatGptServiceCollectionExtensions.md) | Provides extension methods for adding ChatGPT support in .NET applications. |
| interface [IChatGptBuilder](./ChatGptNet/IChatGptBuilder.md) | Represents a builder for configuring ChatGPT. |
| static class [IChatGptBuilderExtensions](./ChatGptNet/IChatGptBuilderExtensions.md) | Provides extension methods for configuring ChatGPT after service creation. |
| interface [IChatGptCache](./ChatGptNet/IChatGptCache.md) | Represents the interface used to define the caching behavior for ChatGPT messages. |
| interface [IChatGptClient](./ChatGptNet/IChatGptClient.md) | Provides methods to interact with ChatGPT. |

## ChatGptNet.Exceptions namespace

| public type | description |
| --- | --- |
| class [ChatGptException](./ChatGptNet.Exceptions/ChatGptException.md) | Represents errors that occur during ChatGPT API invocation. |
| class [EmbeddingException](./ChatGptNet.Exceptions/EmbeddingException.md) | Represents errors that occur during Embeddings API invocation. |

## ChatGptNet.Extensions namespace

| public type | description |
| --- | --- |
| static class [ChatGptChoiceExtensions](./ChatGptNet.Extensions/ChatGptChoiceExtensions.md) | Contains extension methods for the [`ChatGptChoice`](./ChatGptNet.Models/ChatGptChoice.md) class. |
| static class [ChatGptFunctionExtensions](./ChatGptNet.Extensions/ChatGptFunctionExtensions.md) | Provides extension methods for working with [`ChatGptFunction`](./ChatGptNet.Models/ChatGptFunction.md) instances. |
| static class [ChatGptResponseExtensions](./ChatGptNet.Extensions/ChatGptResponseExtensions.md) | Provides extension methods for the [`ChatGptResponse`](./ChatGptNet.Models/ChatGptResponse.md) class. |
| static class [EmbeddingResponseExtensions](./ChatGptNet.Extensions/EmbeddingResponseExtensions.md) | Provides extension methods for the [`EmbeddingResponse`](./ChatGptNet.Models.Embeddings/EmbeddingResponse.md) class. |
| static class [EmbeddingUtility](./ChatGptNet.Extensions/EmbeddingUtility.md) | Provides utility methods to work with embeddings. |

## ChatGptNet.Models namespace

| public type | description |
| --- | --- |
| class [ChatGptChoice](./ChatGptNet.Models/ChatGptChoice.md) | Represent a chat completion choice. |
| class [ChatGptContentFilterError](./ChatGptNet.Models/ChatGptContentFilterError.md) | Contains information about the error occurred in the content filtering system. |
| class [ChatGptContentFilterResult](./ChatGptNet.Models/ChatGptContentFilterResult.md) | Contains detail about a particular content filter result. |
| class [ChatGptContentFilterResults](./ChatGptNet.Models/ChatGptContentFilterResults.md) | Contains details about conteng filtering results. |
| static class [ChatGptContentFilterSeverityLevels](./ChatGptNet.Models/ChatGptContentFilterSeverityLevels.md) | Contains all the content filter severity levels defined by Azure OpenAI Service. |
| class [ChatGptError](./ChatGptNet.Models/ChatGptError.md) | Contains information about the error occurred while invoking the service. |
| static class [ChatGptFinishReasons](./ChatGptNet.Models/ChatGptFinishReasons.md) | Contains constants for all the possible chat completion finish reasons. |
| class [ChatGptFunction](./ChatGptNet.Models/ChatGptFunction.md) | Represents the description of a function available for ChatGPT. |
| class [ChatGptFunctionCall](./ChatGptNet.Models/ChatGptFunctionCall.md) | Represents a response function call. |
| class [ChatGptInnerError](./ChatGptNet.Models/ChatGptInnerError.md) | Contains further details about the error. |
| class [ChatGptMessage](./ChatGptNet.Models/ChatGptMessage.md) | Represents a single chat message. |
| class [ChatGptParameters](./ChatGptNet.Models/ChatGptParameters.md) | Represents chat completion parameters. |
| class [ChatGptPromptFilterResults](./ChatGptNet.Models/ChatGptPromptFilterResults.md) | Contains information about content filtering for input prompts. |
| class [ChatGptResponse](./ChatGptNet.Models/ChatGptResponse.md) | Represents a chat completion response. |
| class [ChatGptResponseFormat](./ChatGptNet.Models/ChatGptResponseFormat.md) | An object specifying the format that the model must output. Used to enable JSON mode. |
| static class [ChatGptResponseFormatTypes](./ChatGptNet.Models/ChatGptResponseFormatTypes.md) | Contains constants for all the possible chat completion response formats. |
| static class [ChatGptRoles](./ChatGptNet.Models/ChatGptRoles.md) | Contains constants for all the possible roles. |
| class [ChatGptTool](./ChatGptNet.Models/ChatGptTool.md) | Represents a tool that the model may call. |
| class [ChatGptToolCall](./ChatGptNet.Models/ChatGptToolCall.md) | A tool call generated by the model, such as a function call. |
| static class [ChatGptToolChoices](./ChatGptNet.Models/ChatGptToolChoices.md) | Contains constants for ChatGPT function call types. |
| class [ChatGptToolParameters](./ChatGptNet.Models/ChatGptToolParameters.md) | Contains parameters about the tools calls that are available for ChatGPT. |
| static class [ChatGptToolTypes](./ChatGptNet.Models/ChatGptToolTypes.md) | Contains constants for ChatGPT tool types. |
| class [ChatGptUsage](./ChatGptNet.Models/ChatGptUsage.md) | Contains information about the API usage. |
| static class [OpenAIChatGptModels](./ChatGptNet.Models/OpenAIChatGptModels.md) | Contains all the chat completion models that are currently supported by OpenAI. |

## ChatGptNet.Models.Common namespace

| public type | description |
| --- | --- |
| abstract class [Response](./ChatGptNet.Models.Common/Response.md) | Contains common properties for all response types. |

## ChatGptNet.Models.Embeddings namespace

| public type | description |
| --- | --- |
| class [EmbeddingData](./ChatGptNet.Models.Embeddings/EmbeddingData.md) | Represents an embedding. |
| class [EmbeddingResponse](./ChatGptNet.Models.Embeddings/EmbeddingResponse.md) | Represents an embedding response. |
| static class [OpenAIEmbeddingModels](./ChatGptNet.Models.Embeddings/OpenAIEmbeddingModels.md) | Contains all the embedding models that are currently supported by OpenAI. |

## ChatGptNet.ServiceConfigurations namespace

| public type | description |
| --- | --- |
| enum [AzureAuthenticationType](./ChatGptNet.ServiceConfigurations/AzureAuthenticationType.md) | Enumerates the available Azure authentication types for OpenAI service. |

<!-- DO NOT EDIT: generated by xmldocmd for ChatGptNet.dll -->
