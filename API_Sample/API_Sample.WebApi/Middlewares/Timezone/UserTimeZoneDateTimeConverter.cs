using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace API_Sample.WebApi.Middlewares.Timezone
{
    public sealed class UserTimeZoneDateTimeConverter : JsonConverter<DateTime>
    {
        private readonly IUserTimeZoneProvider _userTimeZoneProvider;

        public UserTimeZoneDateTimeConverter(IUserTimeZoneProvider userTimeZoneProvider)
        {
            _userTimeZoneProvider = userTimeZoneProvider;
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Không tự convert bừa ở input.
            // Chỉ parse theo dữ liệu client gửi lên.
            if (reader.TokenType == JsonTokenType.String)
            {
                var raw = reader.GetString();

                if (string.IsNullOrWhiteSpace(raw))
                    return default;

                // Nếu có Z hoặc offset => parse chuẩn
                if (DateTimeOffset.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dto))
                {
                    return dto.UtcDateTime;
                }

                // Nếu không có offset => coi là UTC để tránh lệch server local
                if (DateTime.TryParse(raw, CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                        out var dt))
                {
                    return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                }
            }

            throw new JsonException("Invalid DateTime format.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var utc = value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),
                _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
            };

            var tz = _userTimeZoneProvider.GetTimeZoneInfo();
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(utc, tz);
            var offset = tz.GetUtcOffset(utc);
            var dto = new DateTimeOffset(localTime, offset);

            writer.WriteStringValue(dto.ToString("yyyy-MM-ddTHH:mm:sszzz"));
        }
    }
}
