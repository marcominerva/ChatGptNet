namespace ChatGptNet.Models;

/// <summary>
/// Contains all the currently supported chat completion models.
/// </summary>
public static class ChatGptModels
{
    /// <summary>
    /// The model used by the official ChatGPT.
    /// </summary>
    public const string Gpt35Turbo = "gpt-3.5-turbo";
    /// <summary>
    /// The latest model by OpenAI: currently in a limited beta and only accessible to those who have been granted access.
    /// </summary>
    public const string Gpt4 = "gpt-4";
}