using ChatGptNet.Models.Embeddings;

namespace ChatGptNet.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="EmbeddingResponse"/> class.
/// </summary>
public static class EmbeddingResponseExtensions
{
    /// <summary>
    /// Gets the first embedding data, if availably.
    /// </summary>
    /// <returns>The first embedding data, if available.</returns>
    public static float[]? GetEmbedding(this EmbeddingResponse response)
        => response.Data?.FirstOrDefault()?.Embedding;
}
