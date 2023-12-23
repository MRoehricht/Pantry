using Microsoft.AspNetCore.Http;

namespace Pantry.Services.UserServices {
    public class HeaderEMailService : IHeaderEMailService {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HeaderEMailService(IHttpContextAccessor httpContextAccessor) {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetHeaderEMail() {
            return _httpContextAccessor.HttpContext.Request.Headers["UserEMail"];
        }
    }
}
