using System.Text;
using System.Text.Json;
using ChatGptNet;
using ChatGptNet.Extensions;
using ChatGptNet.Models;

namespace ChatGptConsole;

internal class Application(IChatGptClient chatGptClient)
{
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
                            "description": "The temperature unit to use. Infer this from the user's location."
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
                            "description": "The temperature unit to use. Infer this from the user's location."
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

        var toolParameters = new ChatGptToolParameters
        {
            FunctionCall = ChatGptToolChoices.Auto,   // This is the default if functions are present.
            Functions = functions
        };

        // If the you're using a recent model that supports tool calls (a more generic approach to function calling),
        // for example the gpt-4 1106-preview model, you can use the following code instead:
        //var toolParameters = new ChatGptToolParameters
        //{
        //    ToolChoice = ChatGptToolChoices.Auto,   // This is the default if functions are present.
        //    Tools = functions.ToTools()
        //};

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

                    //var response = await chatGptClient.AskAsync(conversationId, message, toolParameters);
                    ChatGptResponse chatResponse = null;
                    StringBuilder argument = new StringBuilder();
                    var r = chatGptClient.AskStreamAsync(conversationId, message, null, toolParameters);
                    await foreach (var response in r)
                    {
                        /*Keep response*/
                        chatResponse = response;
                        if (response.ContainsFunctionCalls())
                        {
                            Console.Write(response.GetArgument());
                            argument.Append(response.GetArgument());
                        }
                        else
                        {
                            Console.Write(response.GetContent());
                        }
                    }

                    if (chatResponse!.ContainsFunctionCalls())
                    {

                        Console.WriteLine("I have identified a function to call:");

                        var functionCall = chatResponse!.GetFunctionCall()!;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(functionCall.Name);
                        Console.WriteLine(argument.ToString());
                        Console.ResetColor();

                        // Simulates the call to the function.
                        var functionResponse = await GetWeatherAsync(JsonDocument.Parse(argument.ToString()));

                        // After the function has been called, it is necessary to add the response to the conversation.

                        // If you're using the legacy function calling approach, or if you're using a model that doesn't support tool calls,
                        // you need to use the following code:
                        await chatGptClient.AddToolResponseAsync(conversationId, functionCall, functionResponse);

                        // If, instead, you're using a recent model that supports tool calls (a more generic approach to function calling),
                        // for example the gpt-4 1106-preview model, you need the following code:
                        //var tool = response.GetToolCalls()!.First();
                        //await chatGptClient.AddToolResponseAsync(conversationId, tool, functionResponse);

                        Console.WriteLine("The function gives the following response:");

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(functionResponse);
                        Console.ResetColor();

                        // Finally, it sends the original message back to the model, to obtain a response that takes into account the function call.
                        var rsp = chatGptClient.AskStreamAsync(conversationId, message, null, toolParameters);
                        await foreach (var response in r)
                        {
                            Console.Write(response.GetContent());
                        }
                    }

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
        string[] summaries =
        [
            "Freezing",
            "Bracing",
            "Chilly",
            "Cool",
            "Mild",
            "Warm",
            "Balmy",
            "Hot",
            "Sweltering",
            "Scorching"
        ];

        var location = arguments?.RootElement.GetProperty("location").GetString();

        var response = $$"""
            {
                "location": "{{location}}",
                "temperature": {{Random.Shared.Next(-5, 35)}},
                "description": "{{summaries[Random.Shared.Next(summaries.Length)]}}"
            }
            """;

        return Task.FromResult(response);
    }
}
