using ChatGptBlazor.Wasm;
using ChatGptNet;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Adds ChatGPT service and configure options via code.
builder.Services.AddChatGptClientFactory(options =>
{
    // OpenAI.
    //options.UseOpenAI(apiKey: "", organization: "");

    // Azure OpenAI Service.
    options.UseAzure(resourceName: "", apiKey: "", authenticationType: AzureAuthenticationType.ApiKey);

    options.DefaultModel = "my-model";
    options.MessageLimit = 16;  // Default: 10
    options.MessageExpiration = TimeSpan.FromMinutes(5);    // Default: 1 hour   
},
httpClient =>
{
    // Configures retry policy on the inner HttpClient using Polly.
    httpClient.AddStandardResilienceHandler(options =>
    {
        options.AttemptTimeout.Timeout = TimeSpan.FromMinutes(1);
        options.CircuitBreaker.SamplingDuration = TimeSpan.FromMinutes(3);
        options.TotalRequestTimeout.Timeout = TimeSpan.FromMinutes(3);
    });
});

await builder.Build().RunAsync();
