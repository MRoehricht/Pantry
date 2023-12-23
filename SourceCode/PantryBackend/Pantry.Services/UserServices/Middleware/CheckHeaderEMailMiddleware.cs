using Microsoft.AspNetCore.Http;

namespace Pantry.Services.UserServices.Middleware {
    public class CheckHeaderEMailMiddleware {
        private readonly RequestDelegate _next;

        public CheckHeaderEMailMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context) {
            if (!context.Request.Headers.ContainsKey("UserEMail")) {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            await _next(context);
        }
    }
}
