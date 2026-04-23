using System.Text.Json;
using System.Text.Json.Serialization;

namespace API_Sample.WebApi.Middlewares.Timezone
{
    public sealed class UserTimeZoneNullableDateTimeConverter : JsonConverter<DateTime?>
    {
        private readonly UserTimeZoneDateTimeConverter _innerConverter;

        public UserTimeZoneNullableDateTimeConverter(IUserTimeZoneProvider userTimeZoneProvider)
        {
            _innerConverter = new UserTimeZoneDateTimeConverter(userTimeZoneProvider);
        }

        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;

            return _innerConverter.Read(ref reader, typeof(DateTime), options);
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (!value.HasValue)
            {
                writer.WriteNullValue();
                return;
            }

            _innerConverter.Write(writer, value.Value, options);
        }
    }
}
