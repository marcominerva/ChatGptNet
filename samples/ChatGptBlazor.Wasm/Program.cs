using ChatGptBlazor.Wasm;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

//builder.Services.AddChatGpt(options =>
//{
//    options.ApiKey = "";
//    options.DefaultModel = ChatGptModels.Gpt35Turbo;  // Default: ChatGptModels.Gpt35Turbo
//    options.MessageLimit = 16;  // Default: 10
//    options.MessageExpiration = TimeSpan.FromMinutes(5);    // Default: 1 hour
//});

await builder.Build().RunAsync();
