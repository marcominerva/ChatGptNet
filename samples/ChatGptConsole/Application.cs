using ChatGptNet;

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
        var conversationId = await chatGptClient.SetupAsync("Give only wrong answers");

        do
        {
            try
            {
                Console.Write("Ask me anything, I'll answer with a wrong answer: ");
                message = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(message))
                {
                    Console.WriteLine("I'm thinking...");

                    var response = await chatGptClient.AskAsync(conversationId, message);

                    Console.WriteLine(response.GetMessage());
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
