using Microsoft.Extensions.Primitives;

namespace API_Sample.WebApi.Middlewares
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public Task Invoke(HttpContext context)
        {
            try
            {
                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Referrer-Policy
                // TODO Change the value depending of your needs
                context.Response.Headers.Add("referrer-policy", new StringValues("strict-origin-when-cross-origin"));

                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Content-Type-Options
                context.Response.Headers.Add("x-content-type-options", new StringValues("nosniff"));

                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options
                context.Response.Headers.Add("x-frame-options", new StringValues("DENY"));

                // https://security.stackexchange.com/questions/166024/does-the-x-permitted-cross-domain-policies-header-have-any-benefit-for-my-websit
                context.Response.Headers.Add("X-Permitted-Cross-Domain-Policies", new StringValues("none"));

                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-XSS-Protection
                context.Response.Headers.Add("x-xss-protection", new StringValues("1; mode=block"));

                context.Response.Headers.Add("Access-Control-Allow-Origin", new StringValues("*"));

                //Required HSTS (https)
                context.Response.Headers.Add("Strict-Transport-Security", new StringValues("max-age=31536000"));
                
                //Restrict content
                context.Response.Headers.Add("Content-Security-Policy", new StringValues("default-src https"));

                //Disable permission access from client
                context.Response.Headers.Add("Feature-Policy", new StringValues("camera 'none'; geolocation 'none'; microphone 'none'; usb 'none';"));
                context.Response.Headers.Add("Permissions-Policy", new StringValues("accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()"));


                //Remove x-powered-by
                context.Response.Headers.Remove("Server");
                context.Response.Headers.Remove("X-Powered-By");
                context.Response.Headers.Remove("X-SourceFiles");
            }
            catch (Exception)
            {
            }
            return _next(context);
        }
    }
}
