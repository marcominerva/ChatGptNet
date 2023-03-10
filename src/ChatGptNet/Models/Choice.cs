namespace ChatGptNet.Models;

public class Choice
{
    public int Index { get; set; }

    public Message Message { get; set; } = new();

    public string FinishReason { get; set; } = string.Empty;
}
