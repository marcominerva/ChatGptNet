using ChatGptNet;
using ChatGptNet.Extensions;
using ChatGptNet.Models;

namespace ChatGptConsole;

internal class Application(IChatGptClient chatGptClient)
{
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

                    var response = await chatGptClient.AskAsync(conversationId, message, new ChatGptParameters
                    {
                        MaxTokens = 150,
                        Temperature = 0.7
                    });

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
}
