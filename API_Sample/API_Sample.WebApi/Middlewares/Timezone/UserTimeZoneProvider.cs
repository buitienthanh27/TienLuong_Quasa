using API_Sample.Utilities.Constants;

namespace API_Sample.WebApi.Middlewares.Timezone
{

    public interface IUserTimeZoneProvider
    {
        string GetTimeZoneId();
        TimeZoneInfo GetTimeZoneInfo();
    }

    public sealed class UserTimeZoneProvider : IUserTimeZoneProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserTimeZoneProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetTimeZoneId()
        {
            var context = _httpContextAccessor.HttpContext;

            var tz = context?.Items["__USER_TIMEZONE_ID__"]?.ToString();

            return string.IsNullOrWhiteSpace(tz)
                ? "SE Asia Standard Time"
                : tz;
        }

        public TimeZoneInfo GetTimeZoneInfo()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(GetTimeZoneId());
            }
            catch
            {
                return TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            }
        }
    }
}
