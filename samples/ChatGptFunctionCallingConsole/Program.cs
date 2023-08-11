using ChatGptConsole;
using ChatGptNet;
using ChatGptNet.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder().ConfigureServices(ConfigureServices)
    .Build();

var application = host.Services.GetRequiredService<Application>();
await application.ExecuteAsync();

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    services.AddSingleton<Application>();

    // Adds ChatGPT service using settings from IConfiguration.
    services.AddChatGpt(context.Configuration)
    //.WithCache<LocalMessageCache>() // Uncomment this line to use a custom cache implementation instead of the default MemoryCache.
    ;

    // Adds ChatGPT service and configure options via code.
    //services.AddChatGpt(options =>
    //{
    //    // OpenAI.
    //    //options.UseOpenAI(apiKey: "", organization: "");

    //    // Azure OpenAI Service.
    //    //options.UseAzure(resourceName: "", apiKey: "", authenticationType: AzureAuthenticationType.ApiKey);

    //    options.DefaultModel = "my-model";
    //    options.MessageLimit = 16;  // Default: 10
    //    options.MessageExpiration = TimeSpan.FromMinutes(5);    // Default: 1 hour
    //});

    // Adds ChatGPT service using settings from IConfiguration and code.
    //services.AddChatGpt(options =>
    //{
    //    options.UseConfiguration(context.Configuration);

    //    options.UseOpenAI(apiKey: "");
    //    options.DefaultModel = OpenAIChatGptModels.Gpt35Turbo;
    //});
}

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