namespace Mango.Web.Services
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _httpContext;
        private const string CookieKey = "JWT";

        public TokenProvider(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public void ClearToken()
        {
            _httpContext.HttpContext?.Response.Cookies.Delete(CookieKey);
        }

        public string? GetToken()
        {
            string? token = null;
            var hasToken = _httpContext.HttpContext?.Request.Cookies.TryGetValue(CookieKey, out token);
            return hasToken == true ? token : null;
        }

        public void SetToken(string? token)
        {
            _httpContext.HttpContext?.Response.Cookies.Append(CookieKey, token);
        }
    }
}
