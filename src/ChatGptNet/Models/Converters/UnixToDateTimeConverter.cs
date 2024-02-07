using System.Buffers;
using System.Buffers.Text;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChatGptNet.Models.Converters;

internal class UnixToDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                // try to parse number directly from bytes
                var span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
                if (Utf8Parser.TryParse(span, out long number, out var bytesConsumed) && span.Length == bytesConsumed)
                {
                    var date = DateTimeOffset.FromUnixTimeSeconds(number).UtcDateTime;
                    return date;
                }
            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                var value = reader.GetString();
                if (DateTime.TryParseExact(value, "O", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var date))
                {
                    return date;
                }
            }
        }
        catch
        {
        }

        return DateTime.MinValue;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        // The "O" standard format will always be 28 bytes.
        Span<byte> utf8Date = new byte[28];

        Utf8Formatter.TryFormat(value, utf8Date, out _, new StandardFormat('O'));
        writer.WriteStringValue(utf8Date);
    }
}
