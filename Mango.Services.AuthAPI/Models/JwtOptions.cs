namespace Mango.Services.AuthAPI.Models
{
    public class JwtOptions
    {
        public string PrivateKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
