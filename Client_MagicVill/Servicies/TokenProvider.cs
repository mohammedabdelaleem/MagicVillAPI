
using MagicVilla_Utility;
using Microsoft.AspNetCore.Mvc;

namespace Client_MagicVill.Servicies;

public class TokenProvider : ITokenProvider
{

	// when you have to work with cookies or sessions	

	private readonly IHttpContextAccessor _httpContextAccessor;
	public TokenProvider(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}

	public void ClearToken()
	{
		_httpContextAccessor.HttpContext?.Response.Cookies.Delete(SD.AccessToken);
	}

	public TokenDTO GetToken()
	{
		try
		{
			bool hasAccessToken = _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(SD.AccessToken, out string accessToken);
			TokenDTO token = new TokenDTO
			{
				AccessToken = accessToken,
			};

			return hasAccessToken ? token : null;	
		}
		catch (Exception ex)
		{
			return null;
		}
	}

	public void SetToken(TokenDTO tokenDTO)
	{
		var cookieOptions = new CookieOptions { Expires = DateTime.UtcNow.AddDays(22) };
		_httpContextAccessor.HttpContext?.Response.Cookies.Append(SD.AccessToken, tokenDTO.AccessToken,	cookieOptions);

	}
}
