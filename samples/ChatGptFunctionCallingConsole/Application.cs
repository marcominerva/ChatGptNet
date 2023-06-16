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
        string? message = null;
        var conversationId = Guid.NewGuid();

        Console.WriteLine("How should the assistant behave?");
        Console.Write("For example: 'You are an helpful assistant', 'Answer like Shakespeare', 'Give me only wrong answers'. (Press ENTER for no recommendation): ");
        var systemMessage = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(systemMessage))
        {
            await chatGptClient.SetupAsync(conversationId, systemMessage);
        }

        Console.WriteLine();

        do
        {
            try
            {
                Console.Write("Ask me anything: ");
                message = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(message))
                {
                    Console.WriteLine("I'm thinking...");

                    var functions = new List<ChatGptFunction>
                    {
                        new()
                        {
                            Name = "get_current_weather",
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
                            Name = "get_n_day_weather_forecast",
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

                    var response = await chatGptClient.AskAsync(conversationId, message, functionParameters);

                    if (response.IsFunctionCall)
                    {
                        Console.WriteLine("I have identified a function to call:");

                        var functionCall = response.GetFunctionCall()!;

                        Console.ForegroundColor = ConsoleColor.Green;

                        Console.WriteLine(functionCall.Name);
                        Console.WriteLine(functionCall.Arguments?.RootElement.ToString());

                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine(response.GetMessage());
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
}
