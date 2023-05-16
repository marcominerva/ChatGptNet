using ChatGptNet.ServiceConfigurations;

namespace ChatGptNet.Models;

/// <summary>
/// Contains all the chat completion models that are currently supported by OpenAI.
/// </summary>
/// <remarks>
/// See <see href="https://platform.openai.com/docs/models/overview">Models overview</see> for more information.
/// </remarks>
/// <seealso cref="OpenAIChatGptServiceConfiguration"/>
public static class OpenAIChatGptModels
{
    /// <summary>
    /// GPT-3.5 model can understand and generate natural language or code and it is optimized for chat.
    /// </summary>
    /// <remarks>
    /// See <see href="https://platform.openai.com/docs/models/gpt-3-5">GPT-3.5</see> for more information.
    /// </remarks>
    public const string Gpt35Turbo = "gpt-3.5-turbo";

    /// <summary>
    /// GPT-4 is a large multimodal model that can solve difficult problems with greater accuracy than any of our previous models, thanks to its broader general knowledge and advanced reasoning capabilities. is optimized for chat but works well for traditional completions tasks.
    /// </summary>
    /// <remarks>
    /// This model is currently in a limited beta and only accessible to those who have been granted access. See <see href="https://platform.openai.com/docs/models/gpt-4">GPT-4</see> for more information.
    /// </remarks>
    /// <seealso cref="Gpt4_32k"/>
    public const string Gpt4 = "gpt-4";

    /// <summary>
    /// A model with the same capabilities as the base <see cref="Gpt4"/> model, but with 4x the context length.
    /// </summary>
    /// <remarks>
    /// This model is currently in a limited beta and only accessible to those who have been granted access. See <see href="https://platform.openai.com/docs/models/gpt-4">GPT-4</see> for more information.
    /// </remarks>
    /// <seealso cref="Gpt4"/>
    public const string Gpt4_32k = "gpt-4-32k";
}