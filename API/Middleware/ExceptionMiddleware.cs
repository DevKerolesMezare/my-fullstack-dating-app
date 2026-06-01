using System.Net;          // يحتوي على HttpStatusCode مثل 404 و 500
using System.Text.Json;    // لتحويل Object إلى JSON
using API.Errors;          // يحتوي على ApiException

namespace API.Middleware;

public class ExceptionMiddleware(
    RequestDelegate next,                  // يشير للـ Middleware التالية
    ILogger<ExceptionMiddleware> logger,   // لتسجيل الأخطاء
    IHostEnvironment env                   // لمعرفة Development أم Production
)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // مرر الطلب للجزء التالي من الـ Pipeline
            await next(context);
        }

        // إذا حدث Exception في أي مكان بعد هذه الـ Middleware
        catch (Exception ex)
        {
            // تسجيل الخطأ في الـ Logs
            logger.LogError(
                ex,
                "{message}",
                ex.Message
            );

            // سنرجع JSON وليس HTML
            context.Response.ContentType =
                "application/json";

            // HTTP Status Code = 500
            context.Response.StatusCode =
                (int)HttpStatusCode.InternalServerError;

            // إنشاء Error Response
            var response = env.IsDevelopment()

                // أثناء التطوير
                ? new ApiException(
                    context.Response.StatusCode,
                    ex.Message,
                    ex.StackTrace
                )

                // أثناء الإنتاج
                : new ApiException(
                    context.Response.StatusCode,
                    ex.Message,
                    "Internal server error"
                );

            // جعل أسماء الخصائص camelCase
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy =
                    JsonNamingPolicy.CamelCase
            };

            // تحويل ApiException إلى JSON String
            var json = JsonSerializer.Serialize(
                response,
                options
            );

            // إرسال الـ JSON للعميل
            await context.Response.WriteAsync(json);
        }
    }
}