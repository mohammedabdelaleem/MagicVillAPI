using Client_MagicVill.Models;

namespace Client_MagicVill.Servicies;

public class VillaService : BaseService, IVillaService
{
	private readonly IHttpClientFactory _httpClient;
	private string villaApi;

	public VillaService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
	{
		_httpClient = httpClient;
		villaApi = configuration.GetValue<string>("UrlServices:VillaApi")!;
	}

	public Task<T> CreateAsync<T>(VillaCreateDTO dto)
	{
		return SendAsync<T>(
			new ApiRequest
			{
				ApiType = ApiType.POST,
				Url = villaApi + "/api/VillaAPI",
				Data = dto
			}
			);
	}

	public Task<T> DeleteAsync<T>(int id)
	{
		return SendAsync<T>(
		new ApiRequest
		{
			ApiType = ApiType.DELETE,
			Url = villaApi + "/api/VillaAPI/"+id,
		}
		);
	}

	public Task<T> GetAllAsync<T>()
	{
			return SendAsync<T>(
			new ApiRequest
			{
				ApiType = ApiType.GET,
				Url = villaApi + "/api/VillaAPI/all"
			}
			);
	}

	public Task<T> GetAsync<T>(int id)
	{
		return SendAsync<T>(
		new ApiRequest
		{
			ApiType = ApiType.GET,
			Url = villaApi + "/api/VillaAPI/"+id,
		}
		);
	}

	public Task<T> UpdateAsync<T>(VillaUpdateDTO dto)
	{
		return SendAsync<T>(
		new ApiRequest
		{
			ApiType = ApiType.PUT,
			Url = villaApi + "/api/VillaAPI/"+dto.Id,
			Data = dto
		}
		);
	}
}
