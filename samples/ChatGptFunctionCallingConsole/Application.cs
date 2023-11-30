using System.Text.Json;
using ChatGptNet;
using ChatGptNet.Models;

namespace ChatGptConsole;

internal class Application
{
    private readonly IChatGptClient chatGptClient;

    public Application(IChatGptClient chatGptClient)
    {
        this.chatGptClient = chatGptClient;
    }

    public async Task ExecuteAsync()
    {
        Console.WriteLine("Welcome! You can ask me whatever you want, but if you ask me something about the weather, I will probably suggest you to call a function.");

        var conversationId = await chatGptClient.SetupAsync("Don't make assumptions about what values to plug into functions. Ask for clarification if a user request is ambiguous.");

        var functions = new List<ChatGptFunction>
        {
            new()
            {
                Name = "GetCurrentWeather",
                Description = "Get the current weather",
                Parameters = JsonDocument.Parse("""                                        
                {
                    "type": "object",
                    "properties": {
                        "location": {
                            "type": "string",
                            "description": "The city and/or the zip code"
                        },
                        "format": {
                            "type": "string",
                            "enum": ["celsius", "fahrenheit"],
                            "description": "The temperature unit to use. Infer this from the users location."
                        }
                    },
                    "required": ["location", "format"]
                }
                """)
            },
            new()
            {
                Name = "GetWeatherForecast",
                Description = "Get an N-day weather forecast",
                Parameters = JsonDocument.Parse("""                                        
                {
                    "type": "object",
                    "properties": {
                        "location": {
                            "type": "string",
                            "description": "The city and/or the zip code"
                        },
                        "format": {
                            "type": "string",
                            "enum": ["celsius", "fahrenheit"],
                            "description": "The temperature unit to use. Infer this from the users location."
                        },
                        "daysNumber": {
                            "type": "integer",
                            "description": "The number of days to forecast"
                        }
                    },
                    "required": ["location", "format", "daysNumber"]
                }
                """)
            }
        };

        var functionParameters = new ChatGptFunctionParameters
        {
            FunctionCall = ChatGptFunctionCalls.Auto,   // This is the default if functions are present.
            Functions = functions
        };

        string? message = null;
        do
        {
            try
            {
                Console.Write("Ask me anything: ");
                message = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(message))
                {
                    Console.WriteLine("I'm thinking...");

                    var response = await chatGptClient.AskAsync(conversationId, message, functionParameters);

                    while (response.IsFunctionCall)
                    {
                        Console.WriteLine("I have identified a function to call:");

                        var functionCall = response.GetFunctionCall()!;

                        Console.ForegroundColor = ConsoleColor.Green;

                        Console.WriteLine(functionCall.Name);
                        Console.WriteLine(functionCall.Arguments);

                        Console.ResetColor();

                        // Simulate the calling to the function.
                        var functionResponse = await GetWeatherAsync(functionCall.GetArgumentsAsJson());
                        Console.WriteLine(functionResponse);
                        await chatGptClient.AddFunctionResponseAsync(conversationId, functionCall.Name, functionResponse);
                        response = await chatGptClient.ResendConversationAsync(conversationId, functionParameters); // if in case there is a follow up function call involved
                    }

                    Console.WriteLine(response.GetContent());                    

                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(ex.Message);
                Console.WriteLine();

                Console.ResetColor();
            }
        } while (!string.IsNullOrWhiteSpace(message));
    }

    private static Task<string> GetWeatherAsync(JsonDocument? arguments)
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        var location = arguments?.RootElement.GetProperty("location").GetString();

        var response = new[] { new { location = location, weather = "Freezing" } };
        return Task.FromResult( JsonSerializer.Serialize(response));
    }
}
