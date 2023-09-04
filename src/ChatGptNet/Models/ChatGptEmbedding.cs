namespace ChatGptNet.Models;

/// <summary>
/// Represents an embedding.
/// </summary>
public class ChatGptEmbedding
{
    /// <summary>
    /// Gets or sets the index of the embedding.
    /// </summary>
    public int Index { get; set; } = 0;

    /// <summary>
    /// Gets or sets the source object for this response.
    /// </summary>
    public string Object { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the embedding data
    /// </summary>
    public float[] Embedding { get; set; } = Array.Empty<float>();
}