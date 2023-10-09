using System.Text.Json.Serialization;

namespace ChatGptNet.Models.Embeddings;

/// <summary>
/// Represents an embedding.
/// </summary>
public class EmbeddingData
{
    /// <summary>
    /// Gets or sets the index of the embedding.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Gets or sets the source object for this response.
    /// </summary>
    public string Object { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the embedding data.
    /// </summary>
    [JsonPropertyName("embedding")]
    public float[] Embeddings { get; set; } = Array.Empty<float>();
}