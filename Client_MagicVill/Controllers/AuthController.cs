using MagicVilla_Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Transactions;

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
			identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, jwt.Claims.FirstOrDefault(c => c.Type == "nameid").Value));
			identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(c => c.Type == "role").Value));
			identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(c => c.Type == "unique_name" || c.Type == "name")?.Value ?? "User"));

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
		var roles = new List<SelectListItem> {
			new SelectListItem{Text=SD.Admin , Value=SD.Admin},
			new SelectListItem{Text=SD.Customer , Value=SD.Customer}
		};

		ViewBag.Roles = roles;

		RegisterationRequestDTO register = new RegisterationRequestDTO();
		return View(register);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Register(RegisterationRequestDTO request)
	{
		if (string.IsNullOrEmpty(request.Role))
		{
			request.Role = SD.Customer;
		}

		var response = await _authService.RegsisterAsync<ApiResponse>(request);

		if (response != null && response.IsSuccess)
		{
			return RedirectToAction(nameof(Login));
		}

		else
		{
			ModelState.AddModelError("ErrorMessages", string.Join(",", response.ErrorMessages));


			var roles = new List<SelectListItem> {
			new SelectListItem{Text=SD.Admin , Value=SD.Admin},
			new SelectListItem{Text=SD.Customer , Value=SD.Customer}
		};

			ViewBag.Roles = roles;
			return View(request);
		}
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
