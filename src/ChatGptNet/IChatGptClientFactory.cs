namespace ChatGptNet;

/// <summary>
/// Provides methods to create new instances of <see cref="IChatGptClient"/> at runtime
/// </summary>
public interface IChatGptClientFactory
{
    /// <summary>
    /// Creates a new instance of a ChatGptClient configured with the supplied action.
    /// </summary>
    /// <param name="setupAction">The <see cref="Action{IServiceProvider, ChatGptOptions}"/> to configure the provided <see cref="ChatGptOptions"/>.</param>
    /// <returns>A new <see cref="IChatGptClient"/></returns>
    IChatGptClient CreateClient(Action<IServiceProvider, ChatGptOptionsBuilder>? setupAction);

    /// <summary>
    /// Creates a new instance of a ChatGptClient configured with the supplied action.
    /// </summary>
    /// <param name="setupAction">The <see cref="Action{ChatGptOptions}"/> to configure the provided <see cref="ChatGptOptions"/>.</param>
    /// <returns>A new <see cref="IChatGptClient"/></returns>
    IChatGptClient CreateClient(Action<ChatGptOptionsBuilder>? setupAction);

    /// <summary>
    /// Creates a new instance of a ChatGptClient.
    /// </summary>
    /// <returns>A new <see cref="IChatGptClient"/></returns>
    IChatGptClient CreateClient();
}
