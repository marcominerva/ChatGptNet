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

    // Adds ChatGPT service with hard-coded settings.
    //services.AddChatGpt(options =>
    //{
    //    options.MessageLimit = 16;  // Default: 10
    //    options.MessageExpiration = TimeSpan.FromMinutes(5);    // Default: 1 hour
    //    options.DefaultModel = "my-model";

    //    // OpenAI.
    //    //options.UseOpenAI(apiKey: "", organization: "");

    //    // Azure OpenAI Service.
    //    //options.UseAzure(resourceName: "", apiKey: "", authenticationType: AzureAuthenticationType.ApiKey);
    //});

    // Adds ChatGPT service using settings from IConfiguration.
    services.AddChatGpt(context.Configuration);
}
