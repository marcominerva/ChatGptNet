using ChatGptNet.Models.Common;

namespace ChatGptNet.Models.Embeddings;

/// <summary>
/// Represents an embedding response.
/// </summary>
public class EmbeddingResponse : Response
{
    /// <summary>
    /// Gets or sets the array of embedding objects.
    /// </summary>
    public EmbeddingData[]? Data { get; set; } = [];
}