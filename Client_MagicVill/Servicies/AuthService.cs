using Client_MagicVill.Contracts.Auth;
using Client_MagicVill.Servicies.IServicies;
using MagicVilla_Utility;

namespace Client_MagicVill.Servicies;

public class AuthService : IAuthService
{
	private readonly IHttpClientFactory _httpClient;
	private readonly IBaseService _baseService;
	private string villaApi;
	private string version;

	public AuthService(IHttpClientFactory httpClient, IConfiguration configuration, ILogger<BaseService> logger, IBaseService baseService)
	{
		_httpClient = httpClient;
		_baseService = baseService;
		villaApi = configuration.GetValue<string>("UrlServices:VillaApi")!;
		version = SD.CurrentApiVersion;

	}

	public async Task<T> LoginAsync<T>(LoginRequestDTO obj)
	{
		return await _baseService.SendAsync<T>(
	new ApiRequest
	{
		ApiType = ApiType.POST,
		Url = villaApi + $"/auth/{version}/Users/login",
		Data = obj
	}
	, withBearer:false);
	}

	public async Task<T> RegsisterAsync<T>(RegisterationRequestDTO obj)
	{
		return await _baseService.SendAsync<T>(
	new ApiRequest
	{
		ApiType = ApiType.POST,
		Url = villaApi + $"/auth/{version}/Users/register",
		Data = obj
	}
	, withBearer: false);
	}
}
