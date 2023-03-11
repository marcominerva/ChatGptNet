using ChatGptConsole;
using ChatGptNet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder().ConfigureServices(ConfigureServices)
    .Build();

var application = host.Services.GetRequiredService<Application>();
await application.ExecuteAsync();

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    services.AddSingleton<Application>();

    services.AddChatGpt(options =>
    {
        options.ApiKey = "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";
    });
}
