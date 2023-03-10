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
        await chatGptClient.AskAsync("È possibile andare da Milano a Roma in meno di 2 ore?");
        await chatGptClient.AskAsync("Parlami di Taggia");
    }
}
