using ChatGptNet.ServiceConfigurations;

namespace ChatGptNet.Models.Embeddings;

/// <summary>
/// Contains all the embedding models that are currently supported by OpenAI.
/// </summary>
/// <remarks>
/// See <see href="https://platform.openai.com/docs/models/embeddings">Models overview</see> for more information.
/// </remarks>
/// <seealso cref="OpenAIChatGptServiceConfiguration"/>
public static class OpenAIEmbeddingModels
{
    /// <summary>
    /// The second generation embedding model provided by OpenAI. It uses a 1536 output dimension.
    /// </summary>
    public const string TextEmbeddingAda002 = "text-embedding-ada-002";

    /// <summary>
    /// Increased performance over 2nd generation ada embedding model. It uses a 1536 output dimension.
    /// </summary>
    public const string TextEmbedding3Small = "text-embedding-3-small";

    /// <summary>
    /// Most capable embedding model for both english and non-english tasks. It uses a 3072 output dimension.
    /// </summary>
    public const string TextEmbedding3Large = "text-embedding-3-large";
}