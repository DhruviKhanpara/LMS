namespace LMS.Presentation.Middlewares
{
    public class ValidateOriginMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ValidateOriginMiddleware> _logger;

        public ValidateOriginMiddleware(RequestDelegate next, ILogger<ValidateOriginMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var origin = context.Request.Headers["Origin"].ToString();

            var whiteListedOrigin = new[] { "https://localhost:7184", "http://localhost:5262" };

            if (!string.IsNullOrEmpty(origin) && !whiteListedOrigin.Contains(origin))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Invalid origin.");
                _logger.LogError("403 Forbidden: Invalid origin '{Origin}' attempted to access '{Path}'", origin, context.Request.Path);
                return;
            }

            await _next(context);
        }
    }
}