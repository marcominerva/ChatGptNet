# ChatGptLogProbabilityContent.Bytes property

Gets or sets a list of integers representing the UTF-8 bytes representation of the token. Useful in instances where characters are represented by multiple tokens and their byte representations must be combined to generate the correct text representation. Can be `null` if there is no bytes representation for the token.

```csharp
public IEnumerable<byte>? Bytes { get; set; }
```

## See Also

* class [ChatGptLogProbabilityContent](../ChatGptLogProbabilityContent.md)
* namespace [ChatGptNet.Models](../../ChatGptNet.md)

<!-- DO NOT EDIT: generated by xmldocmd for ChatGptNet.dll -->
