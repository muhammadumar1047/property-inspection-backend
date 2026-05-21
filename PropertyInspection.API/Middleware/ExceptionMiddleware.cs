using PropertyInspection.Shared;

namespace PropertyInspection.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}", ex.Message);
                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = 500;

                var response = new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Internal Server Error: {ex.Message} - {ex.InnerException?.Message}\nStack Trace: {ex.StackTrace}",
                    ErrorCode = ServiceErrorCodes.ServerError,
                    Data = null!
                };

                await httpContext.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
