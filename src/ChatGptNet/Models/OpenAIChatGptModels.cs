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
    /// This model supports 4.096 tokens. See <see href="https://platform.openai.com/docs/models/gpt-3-5">GPT-3.5</see> for more information.
    /// </remarks>
    /// <seealso cref="Gpt35_Turbo_16k"/>
    public const string Gpt35_Turbo = "gpt-3.5-turbo";

    /// <summary>
    /// A model with the same capabilities as the standard <see cref="Gpt35_Turbo"/> model but with 4 times the token limit of <see cref="Gpt35_Turbo"/>.
    /// </summary>
    /// <remarks>
    /// This model supports 16.384 tokens. See <see href="https://platform.openai.com/docs/models/gpt-3-5">GPT-3.5</see> for more information.
    /// </remarks>
    /// <seealso cref="Gpt35_Turbo"/>
    public const string Gpt35_Turbo_16k = "gpt-3.5-turbo-16k";

    /// <summary>
    /// GPT-4 is a large multimodal model that can solve difficult problems with greater accuracy than any of our previous models, thanks to its broader general knowledge and advanced reasoning capabilities. is optimized for chat but works well for traditional completions tasks.
    /// </summary>
    /// <remarks>
    /// This model supports 8.192 tokens. See <see href="https://platform.openai.com/docs/models/gpt-4-and-gpt-4-turbo">GPT-4</see> for more information.
    /// </remarks>
    /// <seealso cref="Gpt4_32k"/>
    public const string Gpt4 = "gpt-4";

    /// <summary>
    /// A model with the same capabilities as the base <see cref="Gpt4"/> model but with 4 times the token limit of <see cref="Gpt4"/>.
    /// </summary>
    /// <remarks>
    /// This model supports 32.768 tokens. See <see href="https://platform.openai.com/docs/models/gpt-4-and-gpt-4-turbo">GPT-4</see> for more information.
    /// </remarks>
    /// <seealso cref="Gpt4"/>
    public const string Gpt4_32k = "gpt-4-32k";

    /// <summary>
    /// The latest GPT-4 model with improved instruction following, JSON mode, reproducible outputs, parallel function calling, and more.
    /// </summary>
    /// <remarks>
    /// This model supports 128.000 tokens and returns a maximum of 4.096 outpout tokens. This preview model is not yet suited for production traffic.
    /// See <see href="https://platform.openai.com/docs/models/gpt-4-and-gpt-4-turbo">GPT-4</see> for more information.
    /// </remarks>
    /// <seealso cref="Gpt4"/>
    public const string Gpt4_Turbo_Preview = "gpt-4-turbo-preview";

    /// <summary>
    /// Ability to understand images, in addition to all other GPT-4 Turbo capabilties. 
    /// </summary>
    /// <remarks>
    /// This model supports 128.000 tokens and returns a maximum of 4.096 outpout tokens. This preview model is not yet suited for production traffic.
    /// See <see href="https://platform.openai.com/docs/models/gpt-4-and-gpt-4-turbo">GPT-4</see> for more information.
    /// </remarks>
    /// <seealso cref="Gpt4"/>
    public const string Gpt4Vision_Preview = "gpt-4-vision-preview";
}