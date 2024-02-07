# ChatGptServiceCollectionExtensions.AddChatGpt method (1 of 5)

Registers a ChatGptClient instance with the specified options.

```csharp
public static IChatGptBuilder AddChatGpt(this IServiceCollection services, 
    Action<ChatGptOptionsBuilder> builder, Action<IHttpClientBuilder>? httpClientBuilder = null)
```

| parameter | description |
| --- | --- |
| services | The IServiceCollection to add services to. |
| builder | The [`ChatGptOptionsBuilder`](../ChatGptOptionsBuilder.md) to configure options. |
| httpClientBuilder | The IHttpClientBuilder to configure the HTTP client used to make HTTP requests. |

## Return Value

A [`IChatGptBuilder`](../IChatGptBuilder.md) that can be used to further customize ChatGPT.

## Remarks

This method automatically adds a MemoryCache that is used to save conversation history for chat completion. It is possibile to use [`WithCache`](../IChatGptBuilderExtensions/WithCache.md) to specify another cache implementation.

## See Also

* class [ChatGptOptionsBuilder](../ChatGptOptionsBuilder.md)
* interface [IChatGptBuilder](../IChatGptBuilder.md)
* class [ChatGptServiceCollectionExtensions](../ChatGptServiceCollectionExtensions.md)
* namespace [ChatGptNet](../../ChatGptNet.md)

---

# ChatGptServiceCollectionExtensions.AddChatGpt method (2 of 5)

Registers a ChatGptClient instance using dynamic options.

```csharp
public static IChatGptBuilder AddChatGpt(this IServiceCollection services, 
    Action<IServiceProvider, ChatGptOptionsBuilder> builder, 
    Action<IHttpClientBuilder>? httpClientBuilder = null)
```

| parameter | description |
| --- | --- |
| services | The IServiceCollection to add services to. |
| builder | The [`ChatGptOptionsBuilder`](../ChatGptOptionsBuilder.md) to configure options. |
| httpClientBuilder | The IHttpClientBuilder to configure the HTTP client used to make HTTP requests. |

## Return Value

A [`IChatGptBuilder`](../IChatGptBuilder.md) that can be used to further customize ChatGPT.

## Remarks

Use this this method if it is necessary to dynamically set options (for example, using other services via dependency injection). This method automatically adds a MemoryCache that is used to save conversation history for chat completion. It is possibile to use [`WithCache`](../IChatGptBuilderExtensions/WithCache.md) to specify another cache implementation.

## See Also

* class [ChatGptOptions](../ChatGptOptions.md)
* interface [IChatGptBuilder](../IChatGptBuilder.md)
* class [ChatGptOptionsBuilder](../ChatGptOptionsBuilder.md)
* class [ChatGptServiceCollectionExtensions](../ChatGptServiceCollectionExtensions.md)
* namespace [ChatGptNet](../../ChatGptNet.md)

---

# ChatGptServiceCollectionExtensions.AddChatGpt method (3 of 5)

Registers a ChatGptClient instance reading configuration from the specified IConfiguration source, searching for the ChatGPT section.

```csharp
public static IChatGptBuilder AddChatGpt(this IServiceCollection services, 
    IConfiguration configuration, Action<IHttpClientBuilder>? httpClientBuilder = null)
```

| parameter | description |
| --- | --- |
| services | The IServiceCollection to add services to. |
| configuration | The IConfiguration being bound. |
| httpClientBuilder | The IHttpClientBuilder to configure the HTTP client used to make HTTP requests. |

## Remarks

This method automatically adds a MemoryCache that is used to save conversation history for chat completion. It is possibile to use [`WithCache`](../IChatGptBuilderExtensions/WithCache.md) to specify another cache implementation.

## See Also

* class [ChatGptOptions](../ChatGptOptions.md)
* interface [IChatGptBuilder](../IChatGptBuilder.md)
* class [ChatGptServiceCollectionExtensions](../ChatGptServiceCollectionExtensions.md)
* namespace [ChatGptNet](../../ChatGptNet.md)

---

# ChatGptServiceCollectionExtensions.AddChatGpt method (4 of 5)

Registers a ChatGptClient instance reading configuration from the specified IConfiguration source.

```csharp
public static IChatGptBuilder AddChatGpt(this IServiceCollection services, 
    IConfiguration configuration, string sectionName)
```

| parameter | description |
| --- | --- |
| services | The IServiceCollection to add services to. |
| configuration | The IConfiguration being bound. |
| sectionName | The name of the configuration section that holds ChatGPT settings. |

## Remarks

This method automatically adds a MemoryCache that is used to save conversation history for chat completion. It is possibile to use [`WithCache`](../IChatGptBuilderExtensions/WithCache.md) to specify another cache implementation.

## See Also

* class [ChatGptOptions](../ChatGptOptions.md)
* interface [IChatGptBuilder](../IChatGptBuilder.md)
* class [ChatGptServiceCollectionExtensions](../ChatGptServiceCollectionExtensions.md)
* namespace [ChatGptNet](../../ChatGptNet.md)

---

# ChatGptServiceCollectionExtensions.AddChatGpt method (5 of 5)

Registers a ChatGptClient instance reading configuration from the specified IConfiguration source.

```csharp
public static IChatGptBuilder AddChatGpt(this IServiceCollection services, 
    IConfiguration configuration, string sectionName, 
    Action<IHttpClientBuilder>? httpClientBuilder = null)
```

| parameter | description |
| --- | --- |
| services | The IServiceCollection to add services to. |
| configuration | The IConfiguration being bound. |
| sectionName | The name of the configuration section that holds ChatGPT settings. |
| httpClientBuilder | The IHttpClientBuilder to configure the HTTP client used to make HTTP requests. |

## Return Value

A [`IChatGptBuilder`](../IChatGptBuilder.md) that can be used to further customize ChatGPT.

## Remarks

This method automatically adds a MemoryCache that is used to save conversation history for chat completion. It is possibile to use [`WithCache`](../IChatGptBuilderExtensions/WithCache.md) to specify another cache implementation.

## See Also

* class [ChatGptOptions](../ChatGptOptions.md)
* interface [IChatGptBuilder](../IChatGptBuilder.md)
* class [ChatGptServiceCollectionExtensions](../ChatGptServiceCollectionExtensions.md)
* namespace [ChatGptNet](../../ChatGptNet.md)

<!-- DO NOT EDIT: generated by xmldocmd for ChatGptNet.dll -->
