namespace ChatGptNet.Models.Embeddings;

/// <summary>
/// Represents a request for creating embeddings.
/// </summary>
/// <remarks>
/// See <see href="https://platform.openai.com/docs/api-reference/embeddings/create">Create embeddings (OpenAI)</see> or <see href="https://learn.microsoft.com/en-us/azure/ai-services/openai/how-to/embeddings?tabs=console">Embeddings basics (Azure)</see> for more information.
/// </remarks>
internal class EmbeddingRequest
{
    /// <summary>
    /// Gets or sets the ID of the model to use.
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Gets or sets the messages array to generate embeddings for.
    /// </summary>
    /// <seealso cref="Input"/>
    public IEnumerable<string> Input { get; set; } = Enumerable.Empty<string>();

    public string? User { get; set; }
}