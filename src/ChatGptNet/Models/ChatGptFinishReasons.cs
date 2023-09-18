namespace ChatGptNet.Models;

/// <summary>
/// Contains constants for all the possible chat completion finish reasons.
/// </summary>
public class ChatGptFinishReasons
{
    /// <summary>
    /// API returned complete model output.
    /// </summary>
    public const string Stop = "stop";

    /// <summary>
    /// Incomplete model output due to <em>max_tokens</em> parameter or token limit.
    /// </summary>
    public const string Length = "length";

    /// <summary>
    /// The model decided to call a function.
    /// </summary>
    public const string FunctionCall = "function_call";

    /// <summary>
    /// Omitted content due to a flag from content filters.
    /// </summary>
    public const string ContentFilter = "content_filter";
}

