using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ChatGptNet;

/// <summary>
/// Provides extension methods for configuring ChatGPT after service creation.
/// </summary>
public static class IChatGptBuilderExtensions
{
    /// <summary>
    /// Uses a custom cache implementation for conversation handling.
    /// </summary>
    /// <typeparam name="TImplementation">The implementation of <see cref="IChatGptCache"/> to use.</typeparam>
    /// <param name="builder">The <see cref="IChatGptBuilder"/> object to configure.</param>
    /// <returns>The <see cref="IChatGptBuilder"/> to further customize ChatGPT.</returns>
    /// <remarks><typeparamref name="TImplementation"/> is registered as Singleton.</remarks>
    /// <seealso cref="IChatGptBuilder"/>
    /// <seealso cref="IChatGptCache"/>
    public static IChatGptBuilder WithCache<TImplementation>(this IChatGptBuilder builder)
        where TImplementation : class, IChatGptCache
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.Replace(ServiceDescriptor.Singleton<IChatGptCache, TImplementation>());

        return builder;
    }
}
