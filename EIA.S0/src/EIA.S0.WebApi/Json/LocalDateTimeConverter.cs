using System.Text.Json;
using System.Text.Json.Serialization;

namespace EIA.S0.WebApi.Json;

/// <summary>
/// local datetime.
/// </summary>
public class LocalDateTimeConverter : JsonConverter<DateTime>
{
    private readonly TimeZoneInfo _localZone = TimeZoneInfo.Local;

    /// <summary>
    /// read.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="typeToConvert"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();
        if (string.IsNullOrEmpty(str))
            return default;

        var dt = DateTime.Parse(str, null, System.Globalization.DateTimeStyles.RoundtripKind);
        return TimeZoneInfo.ConvertTime(dt, _localZone);
    }

    /// <summary>
    /// write.
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var local = TimeZoneInfo.ConvertTime(value, _localZone);
        writer.WriteStringValue(local.ToString("yyyy-MM-ddTHH:mm:ss"));
    }
}