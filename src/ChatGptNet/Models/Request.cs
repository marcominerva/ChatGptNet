namespace ChatGptNet.Models;

public class Request
{
    public string Model { get; set; } = string.Empty;

    public Message[] Messages { get; set; } = Array.Empty<Message>();
}
