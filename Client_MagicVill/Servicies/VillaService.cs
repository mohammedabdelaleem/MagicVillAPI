using Client_MagicVill.Models;
using MagicVilla_Utility;

namespace Client_MagicVill.Servicies;

public class VillaService : BaseService, IVillaService
{
	private readonly IHttpClientFactory _httpClient;
	private string villaApi;
	private string version;

	public VillaService(IHttpClientFactory httpClient, IConfiguration configuration, ILogger<BaseService> logger) : base(httpClient,logger)
	{
		_httpClient = httpClient;
		villaApi = configuration.GetValue<string>("UrlServices:VillaApi")!;
		version = SD.CurrentApiVersion;
	}

	public Task<T> CreateAsync<T>(VillaCreateDTO dto, string token)
	{
		return SendAsync<T>(
			new ApiRequest
			{
				ApiType = ApiType.POST,
				Url = villaApi + $"/api/{version}/VillaAPI",
				Data = dto,
				Token = token,
				ContentType = ContentType.MultipartFormData,
			}
			);
	}

	public Task<T> DeleteAsync<T>(int id, string token)
	{
		return SendAsync<T>(
		new ApiRequest
		{
			ApiType = ApiType.DELETE,
			Url = villaApi + $"/api/{version}/VillaAPI/" + id,
			Token = token
		}
		);
	}

	public Task<T> GetAllAsync<T>(string token)
	{
			return SendAsync<T>(
			new ApiRequest
			{
				ApiType = ApiType.GET,
				Url = villaApi + $"/api/{version}/VillaAPI/all",
				Token = token
			}
			);
	}

	public Task<T> GetAsync<T>(int id, string token)
	{
		return SendAsync<T>(
		new ApiRequest
		{
			ApiType = ApiType.GET,
			Url = villaApi + $"/api/{version}/VillaAPI/" + id,
			Token = token
		}
		);
	}

	public Task<T> UpdateAsync<T>(VillaUpdateDTO dto, string token)
	{
		return SendAsync<T>(
		new ApiRequest
		{
			ApiType = ApiType.PUT,
			Url = villaApi + $"/api/{version}/VillaAPI/" + dto.Id,
			Data = dto,
			Token = token,
			ContentType = ContentType.MultipartFormData,
		}
		);
	}
}
