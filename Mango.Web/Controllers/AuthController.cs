using Mango.Web.Models;
using Mango.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;

        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }

        [HttpGet]
        public IActionResult Register()
        {
            var roles = new List<SelectListItem>
            {
                new() { Text = UserRole.Customer, Value = UserRole.Customer },
                new() { Text = UserRole.Admin, Value = UserRole.Admin },
            };
            ViewBag.Roles = roles;

            var request = new RegisterRequestDto();
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            var response = await _authService.RegisterAsync(request);
            if (!response.IsSuccess)
            {
                TempData["Error"] = response.Error;
                var roles = new List<SelectListItem>
                {
                    new() { Text = UserRole.Customer, Value = UserRole.Customer },
                    new() { Text = UserRole.Admin, Value = UserRole.Admin },
                };
                ViewBag.Roles = roles;
                return View(request);
            }

            TempData["Success"] = "Registration Successful for " + response.Result.UserName;
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login()
        {
            var request = new LoginRequestDto();
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            var response = await _authService.LoginAsync(request);
            if (!response.IsSuccess)
            {
                TempData["Error"] = response.Error;
                return View(request);
            }

            await SignInUser(response.Result.Token);
            _tokenProvider.SetToken(response.Result.Token);
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            return View();
        }

        private async Task SignInUser(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaims(jwt.Claims);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
