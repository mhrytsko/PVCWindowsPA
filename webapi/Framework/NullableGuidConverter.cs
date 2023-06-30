using System.Text.Json;
using System.Text.Json.Serialization;

namespace webapi.Framework
{
    public class NullableGuidConverter : JsonConverter<Guid>
    {
        public NullableGuidConverter() : base() { }

        public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return Guid.Empty;

            var strValue = reader.GetString();

            return string.IsNullOrEmpty(strValue) ? Guid.Empty : new Guid(strValue);
        }

        public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
        {
            if (value == Guid.Empty)
                writer.WriteNullValue();
            else
                writer.WriteStringValue(value.ToString());
        }
    }
}
