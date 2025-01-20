using Microsoft.AspNetCore.Http;

namespace Pantry.Module.Authentication.UserServices.Middleware {
    public class CheckHeaderEMailMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context) {
            if (!context.Request.Headers.ContainsKey("UserEMail")) {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            await next(context);
        }
    }
}
