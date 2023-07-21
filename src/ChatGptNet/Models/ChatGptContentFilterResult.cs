namespace ChatGptNet.Models;

/// <summary>
/// Contains detail about a particular content filter result.
/// </summary>
/// <remarks>
/// See <see href="https://learn.microsoft.com/azure/ai-services/openai/concepts/content-filter">Content filtering</see> for more information.
/// </remarks>
public class ChatGptContentFilterResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the content has been filtered.
    /// </summary>
    public bool Filtered { get; set; }

    /// <summary>
    /// Gets or sets the severity levels of the content.
    /// </summary>
    /// <remarks>
    /// Currently supported severity levels are:
    /// <list type = "bullet" >
    ///   <item>
    ///     <term>Safe</term>
    ///     <description>Content may be related to violence, self-harm, sexual, or hate categories but the terms are used in general, journalistic, scientific, medical, and similar professional contexts, which are appropriate for most audiences.</description>
    ///   </item>
    ///   <item>
    ///     <term>Low</term>
    ///     <description>Content that expresses prejudiced, judgmental, or opinionated views, includes offensive use of language, stereotyping, use cases exploring a fictional world (for example, gaming, literature) and depictions at low intensity.</description>
    ///   </item>
    ///   <item>
    ///     <term>Medium</term>
    ///     <description>	Content that uses offensive, insulting, mocking, intimidating, or demeaning language towards specific identity groups, includes depictions of seeking and executing harmful instructions, fantasies, glorification, promotion of harm at medium intensity.</description>
    ///   </item>
    ///   <item>
    ///     <term>High</term>
    ///     <description>Content that displays explicit and severe harmful instructions, actions, damage, or abuse; includes endorsement, glorification, or promotion of severe harmful acts, extreme or illegal forms of harm, radicalization, or non-consensual power exchange or abuse.</description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <seealso cref="ChatGptContentFilterSeverityLevels"/>
    public string Severity { get; set; } = string.Empty;
}