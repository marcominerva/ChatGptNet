using ChatGptBlazor.Server.Data;
using ChatGptNet;
using ChatGptNet.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

builder.Services.AddChatGpt(options =>
{
    options.ApiKey = "";
    options.Organization = null;    // Optional
    options.DefaultModel = ChatGptModels.Gpt35Turbo;  // Default: ChatGptModels.Gpt35Turbo
    options.MessageLimit = 16;  // Default: 10
    options.MessageExpiration = TimeSpan.FromMinutes(5);    // Default: 1 hour
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
