namespace ChatGptNet.Models.Embeddings;

/// <summary>
/// Represents embeddings parameters.
/// </summary>
/// <remarks>
/// See <see href="https://platform.openai.com/docs/api-reference/embeddings/create">Create embeddings</see> for more information.
/// </remarks>
public class EmbeddingParameters
{
    /// <summary>
    /// The number of dimensions the resulting output embeddings should have. Only supported in <c>text-embedding-3</c> and later models.
    /// </summary>
    public int? Dimensions { get; set; }
}
