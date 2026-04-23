using API_Sample.Utilities.Constants;

namespace API_Sample.WebApi.Middlewares.Timezone
{
    public sealed class TimeZoneMiddleware
    {
        private readonly RequestDelegate _next;

        public TimeZoneMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string? timeZoneId = null;

            // 1. Header: X-Timezone: SE Asia Standard Time
            if (context.Request.Headers.TryGetValue(TimeZoneConstants.HeaderName, out var headerValue))
            {
                timeZoneId = headerValue.FirstOrDefault();
            }

            // 2. Query string: ?timezone=SE Asia Standard Time
            if (string.IsNullOrWhiteSpace(timeZoneId)
                && context.Request.Query.TryGetValue("timezone", out var queryValue))
            {
                timeZoneId = queryValue.FirstOrDefault();
            }

            // 3. Claim user, ví dụ: timezone
            if (string.IsNullOrWhiteSpace(timeZoneId))
            {
                timeZoneId = context.User?.Claims?.FirstOrDefault(x => x.Type == "timezone")?.Value;
            }

            // 4. Validate timezone
            if (!string.IsNullOrWhiteSpace(timeZoneId) && IsValidTimeZone(timeZoneId))
            {
                context.Items[TimeZoneConstants.HttpContextItemKey] = timeZoneId!;
            }
            else
            {
                context.Items[TimeZoneConstants.HttpContextItemKey] = TimeZoneConstants.DefaultTimeZoneId;
            }

            await _next(context);
        }

        private static bool IsValidTimeZone(string timeZoneId)
        {
            try
            {
                _ = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
    public static class TimeZoneMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserTimeZone(this IApplicationBuilder app)
        {
            return app.UseMiddleware<TimeZoneMiddleware>();
        }
    }
}
