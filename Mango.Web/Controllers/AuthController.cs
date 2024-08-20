using Mango.Web.Models;
using Mango.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
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
            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Error ?? "Invalid Result";
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
            if (!response.IsSuccess || response.Result == null)
            {
                TempData["Error"] = response.Error ?? "Invalid Result";
                return View(request);
            }

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(response.Result.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,
                jwt.Claims.First(_ => _.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
                jwt.Claims.First(_ => _.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
                jwt.Claims.First(_ => _.Type == JwtRegisteredClaimNames.Name).Value));
            // ClaimTypes.Name used to display User.Identity.Name:
            identity.AddClaim(new Claim(ClaimTypes.Name,
                jwt.Claims.First(_ => _.Type == JwtRegisteredClaimNames.Name).Value));
            // ClaimTypes.Role used to manage Authorization Access:
            identity.AddClaim(new Claim(ClaimTypes.Role,
                jwt.Claims.First(_ => _.Type == "role").Value));

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            _tokenProvider.SetToken(response.Result.Token);
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
