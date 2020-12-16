using System.Linq;
using Microsoft.AspNetCore.Http;

namespace VeloGreen.Users.Api.Verifiers
{
    public class AccessVerifier : IAccessVerifier
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccessVerifier(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool HaveAccess(string type, string value)
        {
            return _httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.User.Claims.Any(x => x.Type.Equals(type) && x.Value.Equals(value));
        }
    }
}
