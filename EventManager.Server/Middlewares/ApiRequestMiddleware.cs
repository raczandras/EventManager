namespace EventManager.Server.Middlewares
{
    public class ApiRequestMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiRequestMiddleware> _logger;

        public ApiRequestMiddleware(RequestDelegate next, ILogger<ApiRequestMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                _logger.LogInformation("Incoming {Method} {Path}{QueryString}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Request.QueryString
                );

                await _next(context);

                _logger.LogInformation("Completed {Method} {Path}{QueryString} with status {StatusCode}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Request.QueryString,
                    context.Response.StatusCode
                );
            }
            catch (Exception ex)
            {
                var request = context.Request;
                _logger.LogError(ex, "Unhandled exception while processing {Method} {Path}{QueryString}",
                request.Method,
                request.Path,
                request.QueryString);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { Message = "An unexpected error occurred." });
            }
        }
    }
}
