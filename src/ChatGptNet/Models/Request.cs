using System.Text.Json.Serialization;

namespace ChatGptNet.Models;

public class Request
{
    public string ModelId { get; set; } = string.Empty;

    [JsonPropertyName("messages")]
    public Message[] Messages { get; set; } = Array.Empty<Message>();
}
