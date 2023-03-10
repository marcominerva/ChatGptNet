namespace ChatGptNet;

public class ChatGptOptions
{
    public string ApiKey { get; set; } = null!;

    public int MessageLimit { get; set; } = 10;
}
