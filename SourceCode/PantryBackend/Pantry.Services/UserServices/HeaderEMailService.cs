using Microsoft.AspNetCore.Http;

namespace Pantry.Services.UserServices;

public class HeaderEMailService(IHttpContextAccessor httpContextAccessor) : IHeaderEMailService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string? GetHeaderEMail()
    {
        return _httpContextAccessor.HttpContext.Request.Headers["UserEMail"];
    }
}
