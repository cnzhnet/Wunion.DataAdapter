using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Wunion.DataAdapter.CodeFirstDemo.Data.Domain;

namespace Wunion.DataAdapter.CodeFirstDemo
{
    public class UserAccountStatusJsonConverter : JsonConverter<UserAccountStatus>
    {
        public override UserAccountStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return (UserAccountStatus)Convert.ToInt32(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, UserAccountStatus value, JsonSerializerOptions options)
        {
            int tmp = (int)value;
            writer.WriteStringValue(tmp.ToString());
        }
    }
}
