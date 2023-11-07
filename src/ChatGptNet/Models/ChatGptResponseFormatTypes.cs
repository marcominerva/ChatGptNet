namespace ChatGptNet.Models;

/// <summary>
/// Contains constants for all the possible chat completion response formats.
/// </summary>
public static class ChatGptResponseFormatTypes
{
    /// <summary>
    /// The default format.
    /// </summary>
    public const string Text = "text";

    /// <summary>
    /// Enables JSON mode. This guarantees that the message the model generates is valid JSON.
    /// </summary>
    /// <remarks>
    /// Note that your system prompt must still instruct the model to produce JSON, and to help ensure you don't forget, the API will throw an error if the string JSON does not appear in your system message. Also note that the message content may be partial (i.e. cut off) if <see cref="ChatGptChoice.FinishReason"/> is <see cref="ChatGptFinishReasons.Length"/>, which indicates the generation exceeded <em>max_tokens</em> or the conversation exceeded the max context length.
    /// </remarks>
    public const string Json = "json_object";
}
