using Client_MagicVill.Models;

namespace Client_MagicVill.Servicies;

public class VillaService : BaseService, IVillaService
{
	private readonly IHttpClientFactory _httpClient;
	private string villaApi;

	public VillaService(IHttpClientFactory httpClient, IConfiguration configuration, ILogger<BaseService> logger) : base(httpClient,logger)
	{
		_httpClient = httpClient;
		villaApi = configuration.GetValue<string>("UrlServices:VillaApi")!;
	}

	public Task<T> CreateAsync<T>(VillaCreateDTO dto, string token)
	{
		return SendAsync<T>(
			new ApiRequest
			{
				ApiType = ApiType.POST,
				Url = villaApi + "/api/VillaAPI",
				Data = dto,
				Token = token
			}
			);
	}

	public Task<T> DeleteAsync<T>(int id, string token)
	{
		return SendAsync<T>(
		new ApiRequest
		{
			ApiType = ApiType.DELETE,
			Url = villaApi + "/api/VillaAPI/"+id,
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
				Url = villaApi + "/api/VillaAPI/all",
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
			Url = villaApi + "/api/VillaAPI/"+id,
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
			Url = villaApi + "/api/VillaAPI/"+dto.Id,
			Data = dto,
			Token = token
		}
		);
	}
}
