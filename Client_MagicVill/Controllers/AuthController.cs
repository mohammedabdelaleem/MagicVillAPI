using MagicVilla_Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Client_MagicVill.Controllers;
public class AuthController : Controller
{
	private readonly IAuthService _authService;

	public AuthController(IAuthService authService)
	{
		_authService = authService;
	}


	public IActionResult Login()
	{
		LoginRequestDTO loginRequestDTO = new LoginRequestDTO();
		return View(loginRequestDTO);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Login(LoginRequestDTO request)
	{
		var response = await _authService.LoginAsync<ApiResponse>(request);

		if (response != null && response.IsSuccess)
		{
			LoginResponsDTO model = JsonConvert.DeserializeObject<LoginResponsDTO>(Convert.ToString(response.Result));

			var handler = new JwtSecurityTokenHandler();
			var jwt = handler.ReadJwtToken(model.Token);



			// Now We Have A Token ====> Means User Is Logged In 
			// problem , when we we logged in but HttpContext doesn't Know That => at some pages we redirect us to the login page for login even if we are
			// But We Need To Tell HttpContxt That It's Logged In 
			var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
			identity.AddClaim(new Claim(ClaimTypes.NameIdentifier , jwt.Claims.FirstOrDefault(c=>c.Type == "nameid").Value));
			identity.AddClaim(new Claim(ClaimTypes.Role , jwt.Claims.FirstOrDefault(c => c.Type == "role").Value));

			var principal = new ClaimsPrincipal(identity);
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);


			HttpContext.Session.SetString(SD.SessionKey, model.Token);
			return RedirectToAction("Index", "Home");
		}
		else
		{
			ModelState.AddModelError("ErrorMessage", string.Join(",", response.ErrorMessages)); ///// join 
			return View(request);
		}
	}

	public IActionResult Register()
	{
		RegisterationRequestDTO register = new RegisterationRequestDTO();
		return View(register);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Register(RegisterationRequestDTO request)
	{
		var response = await _authService.RegsisterAsync<ApiResponse>(request);

		if (response != null && response.IsSuccess)
		{
			return RedirectToAction(nameof(Login));
		}

		return View(request);
	}


	public IActionResult Logout()
	{
		HttpContext.SignOutAsync();
		HttpContext.Session.SetString(SD.SessionKey, "");
		return RedirectToAction("Index", "Home");

	}

	public IActionResult AccessDenied()
	{
		return View();
	}

}
