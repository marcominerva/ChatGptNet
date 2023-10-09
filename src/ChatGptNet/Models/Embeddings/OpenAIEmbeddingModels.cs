using ChatGptNet.ServiceConfigurations;

namespace ChatGptNet.Models;

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
    /// The second generation embedding model provided by OpenAI.
    /// </summary>
    public const string TextEmbeddingAda002 = "text-embedding-ada-002";
}