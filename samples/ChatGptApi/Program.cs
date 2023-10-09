using System.Diagnostics;
using System.Text.Json.Serialization;
using ChatGptNet;
using ChatGptNet.Extensions;
using Microsoft.AspNetCore.Diagnostics;
using MinimalHelpers.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Adds ChatGPT service and configure options via code.
//builder.Services.AddChatGpt(options =>
//{
//    // OpenAI.
//    //options.UseOpenAI(apiKey: "", organization: "");

//    // Azure OpenAI Service.
//    options.UseAzure(resourceName: "", apiKey: "", authenticationType: AzureAuthenticationType.ApiKey);

//    options.DefaultModel = "my-model";
//    options.MessageLimit = 16;  // Default: 10
//    options.MessageExpiration = TimeSpan.FromMinutes(5);    // Default: 1 hour
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
            var problemDetailsService = context.RequestServices.GetRequiredService<IProblemDetailsService>();
            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
            var error = exceptionHandlerFeature?.Error;

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
    async IAsyncEnumerable<string?> Stream()
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

app.MapDelete("/api/chat/{conversationId:guid}", async (Guid conversationId, bool? preserveSetup, IChatGptClient chatGptClient) =>
{
    await chatGptClient.DeleteConversationAsync(conversationId, preserveSetup.GetValueOrDefault());
    return TypedResults.NoContent();
})
.WithOpenApi();

app.MapGet("/api/chat/{conversationId:guid}", async (Guid conversationId, IChatGptClient chatGptClient) =>
{
    var messagges = await chatGptClient.GetConversationAsync(conversationId);
    return TypedResults.Ok(messagges);
})
.WithOpenApi();

app.MapPost("/api/embeddings", async (EmbeddingRequest request, IChatGptClient chatGptClient) =>
{
    var embeddingResponse = await chatGptClient.GenerateEmbeddingAsync(request.Message);
    return TypedResults.Ok(embeddingResponse);
})
.WithOpenApi();

app.Run();

public record class Request(Guid ConversationId, string Message);

public record class EmbeddingRequest(string Message);
