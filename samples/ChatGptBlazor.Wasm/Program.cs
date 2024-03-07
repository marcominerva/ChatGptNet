using ChatGptBlazor.Wasm;
using ChatGptNet;
using ChatGptNet.ServiceConfigurations;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Adds ChatGPT service and configure options via code.
builder.Services.AddChatGptClientFactory();

await builder.Build().RunAsync();
