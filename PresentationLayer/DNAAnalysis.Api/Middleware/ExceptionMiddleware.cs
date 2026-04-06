using System.Net;
using System.Text.Json;
using DNAAnalysis.API.Responses;

namespace DNAAnalysis.API.Middleware
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

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }

            // ✅ الحالة اللي إحنا محتاجينها
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, ex.Message);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                var response = new ApiResponse<string>(
                    new List<string> { ex.Message },
                    "Bad Request"
                );

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }

            // ❌ أي حاجة تانية (Server Error)
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = new ApiResponse<string>(
                    new List<string> { "An unexpected error occurred." },
                    "Server Error"
                );

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }
    }
}