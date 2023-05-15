using System.Diagnostics;
using System.Net.Mime;
using System.Text.Json.Serialization;
using ChatGptNet;
using ChatGptNet.Extensions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.WebUtilities;
using MinimalHelpers.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Adds ChatGPT service with hard-coded settings.
//builder.Services.AddChatGpt(options =>
//{
//    options.MessageLimit = 16;  // Default: 10
//    options.MessageExpiration = TimeSpan.FromMinutes(5);    // Default: 1 hour

//    // OpenAI.
//    //options.DefaultModel = OpenAIChatGptModels.Gpt35Turbo;
//    //options.ServiceConfiguration = new OpenAIChatGptServiceConfiguration
//    //{
//    //    ApiKey = "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"
//    //};

//    // Azure OpenAI Service.
//    //options.DefaultModel = "gpt-4-32k";
//    //options.ServiceConfiguration = new AzureChatGptServiceConfiguration
//    //{
//    //    ResourceName = "baseopenaiservice",
//    //    ApiKey = "55e86520fee046d3ba09af8d44af7d18"
//    //};
//});

// Adds ChatGPT service using settings from IConfiguration.
builder.Services.AddChatGpt(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddMissingSchemas();
});

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["traceId"] = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
    };
});

var app = builder.Build();

// Configures the HTTP request pipeline.
app.UseHttpsRedirection();

if (!app.Environment.IsDevelopment())
{
    // Error handling
    app.UseExceptionHandler(new ExceptionHandlerOptions
    {
        AllowStatusCode404Response = true,
        ExceptionHandler = async (HttpContext context) =>
        {
            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
            var error = exceptionHandlerFeature?.Error;

            if (context.RequestServices.GetService<IProblemDetailsService>() is { } problemDetailsService)
            {
                // Writes as JSON problem details
                await problemDetailsService.WriteAsync(new()
                {
                    HttpContext = context,
                    AdditionalMetadata = exceptionHandlerFeature?.Endpoint?.Metadata,
                    ProblemDetails =
                    {
                        Status = context.Response.StatusCode,
                        Title = error?.GetType().FullName ?? "An error occurred while processing your request",
                        Detail = error?.Message
                    }
                });
            }
            else
            {
                context.Response.ContentType = MediaTypeNames.Text.Plain;
                var message = ReasonPhrases.GetReasonPhrase(context.Response.StatusCode) switch
                {
                    { Length: > 0 } reasonPhrase => reasonPhrase,
                    _ => "An error occurred"
                };

                await context.Response.WriteAsync(message + "\r\n");
                await context.Response.WriteAsync($"Request ID: {Activity.Current?.Id ?? context.TraceIdentifier}");
            }
        }
    });
}

app.UseStatusCodePages();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = string.Empty;
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ChatGPT API v1");
});

app.MapPost("/api/chat/setup", async (Request request, IChatGptClient chatGptClient) =>
{
    var conversationId = await chatGptClient.SetupAsync(request.ConversationId, request.Message);
    return TypedResults.Ok(new { conversationId });
})
.WithOpenApi();

app.MapPost("/api/chat", async (Request request, IChatGptClient chatGptClient) =>
{
    var response = await chatGptClient.AskAsync(request.ConversationId, request.Message);
    return TypedResults.Ok(response);
})
.WithOpenApi();

app.MapGet("/api/chat/stream", (Guid? conversationId, string message, IChatGptClient chatGptClient) =>
{
    async IAsyncEnumerable<string> Stream()
    {
        // Requests a streaming response.
        var responseStream = chatGptClient.AskStreamAsync(conversationId.GetValueOrDefault(), message);

        // Uses the "AsDeltas" extension method to retrieve partial message deltas only.
        await foreach (var delta in responseStream.AsDeltas())
        {
            yield return delta;
            await Task.Delay(50);
        }
    }

    return Stream();
})
.WithOpenApi();

app.MapDelete("/api/chat/{conversationId:guid}", async (Guid conversationId, IChatGptClient chatGptClient) =>
{
    await chatGptClient.DeleteConversationAsync(conversationId);
    return TypedResults.NoContent();
})
.WithOpenApi();

app.MapGet("/api/chat/{conversationId:guid}", async (Guid conversationId, IChatGptClient chatGptClient) =>
{
    var messagges = await chatGptClient.GetConversationAsync(conversationId);
    return TypedResults.Ok(messagges);
})
.WithOpenApi();

app.Run();

public record class Request(Guid ConversationId, string Message);
