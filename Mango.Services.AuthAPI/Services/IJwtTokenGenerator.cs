using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Services
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(IdentityUser user, string roleName);
    }
}
