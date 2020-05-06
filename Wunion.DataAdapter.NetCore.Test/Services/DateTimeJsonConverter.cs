using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wunion.DataAdapter.NetCore.Test
{
    /// <summary>
    /// <see cref="DateTime"/> 类型的 Json 序列化格式转换控制.
    /// </summary>
    public class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }

    /// <summary>
    /// <see cref="DateTime"/> 可空类型的 Json 序列化格式转换控制.
    /// </summary>
    public class DateTimeNullableConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string text = reader.GetString();
            if (string.IsNullOrEmpty(text))
                return null;
            return DateTime.Parse(text);
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value != null)
                writer.WriteStringValue(value.Value.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}
