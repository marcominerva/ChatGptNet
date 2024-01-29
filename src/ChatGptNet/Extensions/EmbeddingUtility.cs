using ChatGptNet.Models.Embeddings;

namespace ChatGptNet.Extensions;

/// <summary>
/// Provides utility methods to work with embeddings.
/// </summary>
public static class EmbeddingUtility
{
    /// <summary>
    /// Computes the cosine similarity between two vectors.
    /// </summary>
    /// <param name="x">The first vector.</param>
    /// <param name="y">The second vector.</param>
    /// <returns>The cosine similarity.</returns>
    public static float CosineSimilarity(ReadOnlySpan<float> x, ReadOnlySpan<float> y)
    {
        float dot = 0, xSumSquared = 0, ySumSquared = 0;

        for (var i = 0; i < x.Length; i++)
        {
            dot += x[i] * y[i];
            xSumSquared += x[i] * x[i];
            ySumSquared += y[i] * y[i];
        }

        return dot / (MathF.Sqrt(xSumSquared) * MathF.Sqrt(ySumSquared));
    }

    /// <summary>
    /// Computes the cosine similarity between the result of an embedding request and another vector.
    /// </summary>
    /// <param name="embeddingResponse">The embedding response.</param>
    /// <param name="y">The other vector.</param>
    /// <returns>The cosine similarity.</returns>
    /// <seealso cref="IChatGptClient.GenerateEmbeddingAsync(string, EmbeddingParameters?, string?, CancellationToken)"/>
    /// <seealso cref="EmbeddingResponse"/>
    public static float CosineSimilarity(this EmbeddingResponse embeddingResponse, ReadOnlySpan<float> y)
        => CosineSimilarity(embeddingResponse.GetEmbedding() ?? [], y);

    /// <summary>
    /// Computes the cosine similarity between the results of two embedding requests.
    /// </summary>
    /// <param name="embeddingResponse">The first embedding response.</param>
    /// <param name="otherResponse">The second embedding response.</param>
    /// <returns>The cosine similarity.</returns>
    /// <seealso cref="IChatGptClient.GenerateEmbeddingAsync(string, EmbeddingParameters?, string?, CancellationToken)"/>
    /// <seealso cref="EmbeddingResponse"/>
    public static float CosineSimilarity(this EmbeddingResponse embeddingResponse, EmbeddingResponse otherResponse)
        => CosineSimilarity(embeddingResponse.GetEmbedding() ?? [], otherResponse.GetEmbedding() ?? []);
}
