using Mango.Web.Models;

namespace Mango.Web.Services
{
    public interface IAuthService
    {
        Task<ResponseDto<UserDto>> RegisterAsync(RegisterRequestDto requestDto);
        Task<ResponseDto<LoginResponseDto>> LoginAsync(LoginRequestDto requestDto);
        Task<ResponseDto<UserDto>> AssignRoleAsync(AssignRoleRequestDto requestDto);

    }
}
