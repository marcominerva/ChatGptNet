# IChatGptClient.AskAsync method (1 of 4)

Requests a new chat interaction.

```csharp
public Task<ChatGptResponse> AskAsync(string message, ChatGptParameters? parameters = null, 
    string? model = null, bool addToConversationHistory = true, 
    CancellationToken cancellationToken = default)
```

| parameter | description |
| --- | --- |
| message | The message. |
| parameters | A [`ChatGptParameters`](../../ChatGptNet.Models/ChatGptParameters.md) object used to override the default completion parameters in the [`DefaultParameters`](../ChatGptOptions/DefaultParameters.md) property. |
| model | The chat completion model to use. If *model* is `null`, then the one specified in the [`DefaultModel`](../ChatGptOptions/DefaultModel.md) property will be used. |
| addToConversationHistory | Set to `true` to add the current chat interaction to the conversation history. |
| cancellationToken | The token to monitor for cancellation requests. |

## Return Value

The chat completion response.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentNullException | *message* is `null`. |
| [ChatGptException](../../ChatGptNet.Exceptions/ChatGptException.md) | An error occurred while calling the API and the [`ThrowExceptionOnError`](../ChatGptOptions/ThrowExceptionOnError.md) is `true`. |

## Remarks

This method automatically starts a new conservation with a random Conversation Id, that will be returned in the [`ChatGptResponse`](../../ChatGptNet.Models/ChatGptResponse.md). Subsequent calls to this method must provide the same [`ConversationId`](../../ChatGptNet.Models/ChatGptResponse/ConversationId.md) value, so that previous messages will be automatically used to continue the conversation.

## See Also

* class [ChatGptResponse](../../ChatGptNet.Models/ChatGptResponse.md)
* class [ChatGptOptions](../ChatGptOptions.md)
* class [ChatGptParameters](../../ChatGptNet.Models/ChatGptParameters.md)
* interface [IChatGptClient](../IChatGptClient.md)
* namespace [ChatGptNet](../../ChatGptNet.md)

---

# IChatGptClient.AskAsync method (2 of 4)

Requests a chat interaction.

```csharp
public Task<ChatGptResponse> AskAsync(Guid conversationId, string message, 
    ChatGptParameters? parameters = null, string? model = null, 
    bool addToConversationHistory = true, CancellationToken cancellationToken = default)
```

| parameter | description |
| --- | --- |
| conversationId | The unique identifier of the conversation, used to automatically retrieve previous messages in the chat history. |
| message | The message. |
| parameters | A  object used to override the default completion parameters in the [`DefaultParameters`](../ChatGptOptions/DefaultParameters.md) property. |
| model | The chat completion model to use. If *model* is `null`, then the one specified in the [`DefaultModel`](../ChatGptOptions/DefaultModel.md) property will be used. |
| addToConversationHistory | Set to `true` to add the current chat interaction to the conversation history. |
| cancellationToken | The token to monitor for cancellation requests. |

## Return Value

The chat completion response.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentNullException | *message* is `null`. |
| [ChatGptException](../../ChatGptNet.Exceptions/ChatGptException.md) | An error occurred while calling the API and the [`ThrowExceptionOnError`](../ChatGptOptions/ThrowExceptionOnError.md) is `true`. |

## See Also

* class [ChatGptResponse](../../ChatGptNet.Models/ChatGptResponse.md)
* class [ChatGptParameters](../../ChatGptNet.Models/ChatGptParameters.md)
* interface [IChatGptClient](../IChatGptClient.md)
* namespace [ChatGptNet](../../ChatGptNet.md)

---

# IChatGptClient.AskAsync method (3 of 4)

Requests a new chat interaction.

```csharp
public Task<ChatGptResponse> AskAsync(string message, 
    ChatGptFunctionParameters? functionParameters, ChatGptParameters? parameters = null, 
    string? model = null, bool addToConversationHistory = true, 
    CancellationToken cancellationToken = default)
```

| parameter | description |
| --- | --- |
| message | The message. |
| functionParameters | A [`ChatGptFunctionParameters`](../../ChatGptNet.Models/ChatGptFunctionParameters.md) object that contains the list of available functions for calling. |
| parameters | A [`ChatGptParameters`](../../ChatGptNet.Models/ChatGptParameters.md) object used to override the default completion parameters in the [`DefaultParameters`](../ChatGptOptions/DefaultParameters.md) property. |
| model | The chat completion model to use. If *model* is `null`, then the one specified in the [`DefaultModel`](../ChatGptOptions/DefaultModel.md) property will be used. |
| addToConversationHistory | Set to `true` to add the current chat interaction to the conversation history. |
| cancellationToken | The token to monitor for cancellation requests. |

## Return Value

The chat completion response.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentNullException | *message* is `null`. |
| [ChatGptException](../../ChatGptNet.Exceptions/ChatGptException.md) | An error occurred while calling the API and the [`ThrowExceptionOnError`](../ChatGptOptions/ThrowExceptionOnError.md) is `true`. |

## Remarks

This method automatically starts a new conservation with a random Conversation Id, that will be returned in the [`ChatGptResponse`](../../ChatGptNet.Models/ChatGptResponse.md). Subsequent calls to this method must provide the same [`ConversationId`](../../ChatGptNet.Models/ChatGptResponse/ConversationId.md) value, so that previous messages will be automatically used to continue the conversation.

The Chat Completions API does not call the function; instead, the model generates JSON that you can use to call the function in your code.

## See Also

* class [ChatGptResponse](../../ChatGptNet.Models/ChatGptResponse.md)
* class [ChatGptOptions](../ChatGptOptions.md)
* class [ChatGptFunctionParameters](../../ChatGptNet.Models/ChatGptFunctionParameters.md)
* class [ChatGptParameters](../../ChatGptNet.Models/ChatGptParameters.md)
* interface [IChatGptClient](../IChatGptClient.md)
* namespace [ChatGptNet](../../ChatGptNet.md)

---

# IChatGptClient.AskAsync method (4 of 4)

Requests a chat interaction.

```csharp
public Task<ChatGptResponse> AskAsync(Guid conversationId, string message, 
    ChatGptFunctionParameters? functionParameters, ChatGptParameters? parameters = null, 
    string? model = null, bool addToConversationHistory = true, 
    CancellationToken cancellationToken = default)
```

| parameter | description |
| --- | --- |
| conversationId | The unique identifier of the conversation, used to automatically retrieve previous messages in the chat history. |
| message | The message. |
| functionParameters | A [`ChatGptFunctionParameters`](../../ChatGptNet.Models/ChatGptFunctionParameters.md) object that contains the list of available functions for calling. |
| parameters | A  object used to override the default completion parameters in the [`DefaultParameters`](../ChatGptOptions/DefaultParameters.md) property. |
| model | The chat completion model to use. If *model* is `null`, then the one specified in the [`DefaultModel`](../ChatGptOptions/DefaultModel.md) property will be used. |
| addToConversationHistory | Set to `true` to add the current chat interaction to the conversation history. |
| cancellationToken | The token to monitor for cancellation requests. |

## Return Value

The chat completion response.

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentNullException | *message* is `null`. |
| [ChatGptException](../../ChatGptNet.Exceptions/ChatGptException.md) | An error occurred while calling the API and the [`ThrowExceptionOnError`](../ChatGptOptions/ThrowExceptionOnError.md) is `true`. |

## Remarks

The Chat Completions API does not call the function; instead, the model generates JSON that you can use to call the function in your code.

## See Also

* class [ChatGptResponse](../../ChatGptNet.Models/ChatGptResponse.md)
* class [ChatGptFunctionParameters](../../ChatGptNet.Models/ChatGptFunctionParameters.md)
* class [ChatGptParameters](../../ChatGptNet.Models/ChatGptParameters.md)
* interface [IChatGptClient](../IChatGptClient.md)
* namespace [ChatGptNet](../../ChatGptNet.md)

<!-- DO NOT EDIT: generated by xmldocmd for ChatGptNet.dll -->
