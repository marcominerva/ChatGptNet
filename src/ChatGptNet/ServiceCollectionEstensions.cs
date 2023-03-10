using Microsoft.Extensions.DependencyInjection;

namespace ChatGptNet;

public static class ServiceCollectionEstensions
{
    public static IServiceCollection AddChatGpt(this IServiceCollection services, Action<ChatGptOptions> configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var options = new ChatGptOptions();
        configuration.Invoke(options);
        services.AddSingleton(options);

        services.AddHttpClient<IChatGptClient, ChatGptClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.openai.com/v1/");
            client.DefaultRequestHeaders.Authorization = new("Bearer", options.ApiKey);
        });

        return services;
    }
}
