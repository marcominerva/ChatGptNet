using System.Text.Json.Serialization;

namespace ChatGptNet.Models;

/// <summary>
/// Contains details about conteng filtering results.
/// </summary>
/// <remarks>
/// See <see href="https://learn.microsoft.com/azure/ai-services/openai/concepts/content-filter">Content filtering</see> for more information.
/// </remarks>
public class ChatGptContentFilterResults
{
    /// <summary>
    /// Gets of sets information about the hate content filter.
    /// </summary>
    /// <remarks>
    /// The hate category describes language attacks or uses that include pejorative or discriminatory language with reference to a person or identity group on the basis of certain differentiating attributes of these groups including but not limited to race, ethnicity, nationality, gender identity and expression, sexual orientation, religion, immigration status, ability status, personal appearance, and body size.
    /// </remarks>
    public ChatGptContentFilterResult Hate { get; set; } = new();

    /// <summary>
    /// Gets of sets information about the self-harm content filter.
    /// </summary>
    /// <remarks>
    /// The self-harm category describes language related to physical actions intended to purposely hurt, injure, or damage one’s body, or kill oneself.
    /// </remarks>
    [JsonPropertyName("self_harm")]
    public ChatGptContentFilterResult SelfHarm { get; set; } = new();

    /// <summary>
    /// Gets of sets information about the sexual content filter.
    /// </summary>
    /// <remarks>
    /// The sexual category describes language related to anatomical organs and genitals, romantic relationships, acts portrayed in erotic or affectionate terms, physical sexual acts, including those portrayed as an assault or a forced sexual violent act against one's will, prostitution, pornography, and abuse.
    /// </remarks>
    public ChatGptContentFilterResult Sexual { get; set; } = new();

    /// <summary>
    /// Gets or sets information about the violence content filter.
    /// </summary>
    /// <remarks>
    /// The sexual category describes language related to anatomical organs and genitals, romantic relationships, acts portrayed in erotic or affectionate terms, physical sexual acts, including those portrayed as an assault or a forced sexual violent act against one's will, prostitution, pornography, and abuse.
    /// </remarks>
    public ChatGptContentFilterResult Violence { get; set; } = new();
}
