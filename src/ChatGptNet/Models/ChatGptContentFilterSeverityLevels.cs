namespace ChatGptNet.Models;

/// <summary>
/// Contains all the content filter severity levels defined by Azure OpenAI Service.
/// </summary>
/// <remarks>
/// See <see href="https://learn.microsoft.com/azure/ai-services/openai/concepts/content-filter#severity-levels">Content filtering</see> for more information.
/// </remarks>
public static class ChatGptContentFilterSeverityLevels
{
    /// <summary>
    /// Content may be related to violence, self-harm, sexual, or hate categories but the terms are used in general, journalistic, scientific, medical, and similar professional contexts, which are appropriate for most audiences.
    /// </summary>
    public const string Safe = "Save";

    /// <summary>
    /// Content that expresses prejudiced, judgmental, or opinionated views, includes offensive use of language, stereotyping, use cases exploring a fictional world (for example, gaming, literature) and depictions at low intensity.
    /// </summary>
    public const string Low = "Low";

    /// <summary>
    /// Content that uses offensive, insulting, mocking, intimidating, or demeaning language towards specific identity groups, includes depictions of seeking and executing harmful instructions, fantasies, glorification, promotion of harm at medium intensity.
    /// </summary>
    public const string Medium = "Medium";

    /// <summary>
    /// Content that displays explicit and severe harmful instructions, actions, damage, or abuse; includes endorsement, glorification, or promotion of severe harmful acts, extreme or illegal forms of harm, radicalization, or non-consensual power exchange or abuse.
    /// </summary>
    public const string High = "High";
}
