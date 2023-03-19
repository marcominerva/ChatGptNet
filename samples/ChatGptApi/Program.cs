using System.Diagnostics;
using System.Net.Mime;
using System.Text.Json.Serialization;
using ChatGptNet;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.WebUtilities;
using MinimalHelpers.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Add ChatGPT service.
builder.Services.AddChatGpt(options =>
{
    options.ApiKey = "";
    options.MessageLimit = 16;  // Default: 10
    options.MessageExpiration = TimeSpan.FromMinutes(5);    // Default: 1 hour
    options.OpenAIOrganization = null;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

// Configure the HTTP request pipeline.
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
                // Write as JSON problem details
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

app.MapPost("/api/chat/ask", async (Request request, IChatGptClient chatGptClient) =>
{
    var response = await chatGptClient.AskAsync(request.ConversationId, request.Message);
    return TypedResults.Ok(response);
})
.WithOpenApi();

app.MapDelete("/api/chat/delete", async (Guid conversationId, IChatGptClient chatGptClient) =>
{
    await chatGptClient.DeleteConversationAsync(conversationId);
    return TypedResults.NoContent();
})
.WithOpenApi();

app.Run();

public record class Request(Guid ConversationId, string Message);
