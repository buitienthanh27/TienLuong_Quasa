using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace API_Sample.WebApi.Middlewares.Timezone
{
    public sealed class ConfigureJsonOptions : IConfigureOptions<JsonOptions>
    {
        private readonly IUserTimeZoneProvider _userTimeZoneProvider;

        public ConfigureJsonOptions(IUserTimeZoneProvider userTimeZoneProvider)
        {
            _userTimeZoneProvider = userTimeZoneProvider;
        }

        public void Configure(JsonOptions options)
        {
            options.JsonSerializerOptions.Converters.Add(
                new UserTimeZoneDateTimeConverter(_userTimeZoneProvider));

            options.JsonSerializerOptions.Converters.Add(
                new UserTimeZoneNullableDateTimeConverter(_userTimeZoneProvider));
        }
    }
}
