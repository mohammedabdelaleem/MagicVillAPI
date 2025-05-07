using Client_MagicVill.Contracts.Auth;
using MagicVilla_Utility;

namespace Client_MagicVill.Servicies;

public class AuthService : BaseService, IAuthService
{
	private readonly IHttpClientFactory _httpClient;
	private string villaApi;
	private string version;

	public AuthService(IHttpClientFactory httpClient, IConfiguration configuration, ILogger<BaseService> logger) : base(httpClient, logger)
	{
		_httpClient = httpClient;
		villaApi = configuration.GetValue<string>("UrlServices:VillaApi")!;
		version = SD.Version;

	}

	public Task<T> LoginAsync<T>(LoginRequestDTO obj)
	{
		return SendAsync<T>(
	new ApiRequest
	{
		ApiType = ApiType.POST,
		Url = villaApi + $"/auth/{version}/Users/login",
		Data = obj
	}
	);
	}

	public Task<T> RegsisterAsync<T>(RegisterationRequestDTO obj)
	{
		return SendAsync<T>(
	new ApiRequest
	{
		ApiType = ApiType.POST,
		Url = villaApi + $"/auth/{version}/Users/register",
		Data = obj
	}
	);
	}
}
