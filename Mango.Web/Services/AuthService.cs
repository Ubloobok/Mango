using Mango.Web.Models;

namespace Mango.Web.Services
{
    public class AuthService : IAuthService
    {
        public class Configuration
        {
            public string AuthAPI { get; set; }
        }

        private readonly IBaseService _baseService;
        private readonly Configuration _configuration;

        public AuthService(IBaseService baseService, Configuration configuration)
        {
            _baseService = baseService;
            _configuration = configuration;
        }

        public async Task<ResponseDto<UserDto>> RegisterAsync(RegisterRequestDto requestDto)
        {
            return await _baseService.SendAsync<UserDto>(new RequestDto
            {
                Method = MethodType.POST,
                Authorization = AuthorizationType.None,
                Url = _configuration.AuthAPI + "/api/auth/register",
                Data = requestDto
            });
        }

        public async Task<ResponseDto<LoginResponseDto>> LoginAsync(LoginRequestDto requestDto)
        {
            return await _baseService.SendAsync<LoginResponseDto>(new RequestDto
            {
                Method = MethodType.POST,
                Authorization = AuthorizationType.None,
                Url = _configuration.AuthAPI + "/api/auth/login",
                Data = requestDto
            });
        }

        public async Task<ResponseDto<UserDto>> AssignRoleAsync(AssignRoleRequestDto requestDto)
        {
            return await _baseService.SendAsync<UserDto>(new RequestDto
            {
                Method = MethodType.POST,
                Authorization = AuthorizationType.None,
                Url = _configuration.AuthAPI + "/api/auth/assignRole",
                Data = requestDto
            });
        }
    }
}
