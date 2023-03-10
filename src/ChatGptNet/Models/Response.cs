namespace ChatGptNet.Models;

public class Response
{
    public string Id { get; set; } = string.Empty;

    public string Object { get; set; } = string.Empty;

    public ulong Created { get; set; }

    public Error? Error { get; set; }

    public Choice[] Choices { get; set; } = Array.Empty<Choice>();

    public string? GetMessage() => Choices.FirstOrDefault()?.Message.Content?.Trim();
}
