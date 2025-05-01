using Client_MagicVill.Models;

namespace Client_MagicVill.Servicies;

public class VillaNumberService : BaseService, IVillaNumberService
{
	private readonly IHttpClientFactory _httpClient;
	private string villaNumberApi;

	public VillaNumberService(IHttpClientFactory httpClient, IConfiguration configuration, ILogger<BaseService> logger) : base(httpClient,logger)
	{
		_httpClient = httpClient;
		villaNumberApi = configuration.GetValue<string>("UrlServices:villaApi")!;
	}

	public Task<T> CreateAsync<T>(VillaNumberCreateDTO dto)
	{
		return SendAsync<T>(
			new ApiRequest
			{
				ApiType = ApiType.POST,
				Url = villaNumberApi + "/api/villaNumber",
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
			Url = villaNumberApi + "/api/villaNumber/"+id,
		}
		);
	}
	public Task<T> GetAllAsync<T>()
	{
			return SendAsync<T>(
			new ApiRequest
			{
				ApiType = ApiType.GET,
				Url = villaNumberApi + "/api/villaNumber/all"
			}
			);
	}
	public Task<T> GetAsync<T>(int id)
	{
		return SendAsync<T>(
		new ApiRequest
		{
			ApiType = ApiType.GET,
			Url = villaNumberApi + "/api/villaNumber/"+id,
		}
		);
	}
	public Task<T> UpdateAsync<T>(VillaNumberUpdateDTO dto)
	{
		return SendAsync<T>(
		new ApiRequest
		{
			ApiType = ApiType.PUT,
			Url = villaNumberApi + "/api/villaNumber/"+dto.VillaNo,
			Data = dto
		}
		);
	}
}
