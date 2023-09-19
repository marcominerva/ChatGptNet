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
    /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the service.</param>
    /// <returns>The <see cref="IChatGptBuilder"/> to further customize ChatGPT.</returns>
    /// <remarks><typeparamref name="TImplementation"/> is registered as singleton.</remarks>
    /// <seealso cref="IChatGptBuilder"/>
    /// <seealso cref="IChatGptCache"/>
    public static IChatGptBuilder WithCache<TImplementation>(this IChatGptBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TImplementation : class, IChatGptCache
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.Replace(new ServiceDescriptor(typeof(IChatGptCache), typeof(TImplementation), lifetime));

        return builder;
    }
}
