using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Reflection.Metadata.Ecma335;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthAPIController(AppDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _db = db;
            _userManager = userManager;
            _roleManager=roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        [HttpPost("register")]
        public async Task<ResponseDto<UserDto>> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                if (request.UserName.StartsWith("TEST"))
                {
                    return new()
                    {
                        IsSuccess = false,
                        Error = "TEST user name is not allowed"
                    };
                }

                var user = new IdentityUser
                {
                    UserName = request.UserName,
                    Email = request.Email,
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    return new()
                    {
                        IsSuccess = false,
                        Error = string.Join(";", result.Errors.Select(_ => _.Description))
                    };
                }

                var normalizedRoleName = request.RoleName.ToUpper();
                if (!await _roleManager.RoleExistsAsync(normalizedRoleName))
                {
                    var createResult = await _roleManager.CreateAsync(new IdentityRole(normalizedRoleName));
                    if (!createResult.Succeeded)
                    {
                        return new()
                        {
                            IsSuccess = false,
                            Error = string.Join(";", createResult.Errors.Select(_ => _.Description))
                        };
                    }
                }

                var addResult = await _userManager.AddToRoleAsync(user, normalizedRoleName);
                if (!addResult.Succeeded)
                {
                    return new()
                    {
                        IsSuccess = false,
                        Error = string.Join(";", addResult.Errors.Select(_ => _.Description))
                    };
                }

                var normalizedUserName = user.UserName.ToUpper();
                var createdUser = _db.Users.First(u => u.NormalizedUserName == normalizedUserName);
                return new()
                {
                    IsSuccess = true,
                    Result = new()
                    {
                        UserId = createdUser.Id,
                        UserName = createdUser.UserName,
                        Email = createdUser.Email
                    }
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    IsSuccess = false,
                    Error = ex.Message
                };
            }
        }

        [HttpPost("login")]
        public async Task<ResponseDto<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var normalizedUserName = request.UserName.ToUpper();
                var user = _db.Users.FirstOrDefault(_ => _.NormalizedUserName == normalizedUserName);
                if (user == null)
                {
                    return new()
                    {
                        IsSuccess = false,
                        Error = "Not Found"
                    };
                }

                var isValid = await _userManager.CheckPasswordAsync(user, request.Password);
                if (!isValid)
                {
                    return new()
                    {
                        IsSuccess = false,
                        Error = "Invalid Password"
                    };
                }

                var roles = await _userManager.GetRolesAsync(user);
                string roleName = roles.FirstOrDefault();

                return new()
                {
                    IsSuccess = true,
                    Result = new()
                    {
                        User = new()
                        {
                            UserId = user.Id,
                            UserName = user.UserName,
                            Email = user.Email
                        },
                        Token = _jwtTokenGenerator.GenerateToken(user, roleName)
                    }
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    IsSuccess = false,
                    Error = ex.Message
                };
            }
        }

        [HttpPost("assignRole")]
        public async Task<ResponseDto<UserDto>> AssignRole([FromBody] AssignRoleRequestDto request)
        {
            try
            {
                var normalizedUserName = request.UserName.ToUpper();
                var user = _db.Users.FirstOrDefault(_ => _.NormalizedUserName == normalizedUserName);
                if (user == null)
                {
                    return new()
                    {
                        IsSuccess = false,
                        Error = "Not Found"
                    };
                }

                var normalizedRoleName = request.RoleName.ToUpper();
                if (!await _roleManager.RoleExistsAsync(normalizedRoleName))
                {
                    var createResult = await _roleManager.CreateAsync(new IdentityRole(normalizedRoleName));
                    if (!createResult.Succeeded)
                    {
                        return new()
                        {
                            IsSuccess = false,
                            Error = string.Join(";", createResult.Errors.Select(_ => _.Description))
                        };
                    }
                }

                var addResult = await _userManager.AddToRoleAsync(user, normalizedRoleName);
                if (!addResult.Succeeded)
                {
                    return new()
                    {
                        IsSuccess = false,
                        Error = string.Join(";", addResult.Errors.Select(_ => _.Description))
                    };
                }

                return new()
                {
                    IsSuccess = true,
                    Result = new()
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        Email = user.Email
                    }
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    IsSuccess = false,
                    Error = ex.Message
                };
            }
        }
    }
}
