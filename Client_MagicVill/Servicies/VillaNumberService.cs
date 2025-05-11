using Client_MagicVill.Models;
using MagicVilla_Utility;

namespace Client_MagicVill.Servicies;

public class VillaNumberService : BaseService, IVillaNumberService
{
	private readonly IHttpClientFactory _httpClient;
	private string villaNumberApi;
	private string version;

	public VillaNumberService(IHttpClientFactory httpClient, IConfiguration configuration, ILogger<BaseService> logger) : base(httpClient,logger)
	{
		_httpClient = httpClient;
		villaNumberApi = configuration.GetValue<string>("UrlServices:villaApi")!;
		version = SD.CurrentApiVersion;
	}

	public Task<T> CreateAsync<T>(VillaNumberCreateDTO dto, string token)
	{
		return SendAsync<T>(
			new ApiRequest
			{
				ApiType = ApiType.POST,
				Url = villaNumberApi + $"/api/{version}/villaNumber",
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
			Url = villaNumberApi + $"/api/{version}/villaNumber/"+id,
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
				Url = villaNumberApi + $"/api/{version}/villaNumber/all",
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
			Url = villaNumberApi + $"/api/{version}/villaNumber/" +id,
			Token = token
		}
		);
	}
	public Task<T> UpdateAsync<T>(VillaNumberUpdateDTO dto, string token)
	{
		return SendAsync<T>(
		new ApiRequest
		{
			ApiType = ApiType.PUT,
			Url = villaNumberApi + $"/api/{version}/villaNumber/" +dto.VillaNo,
			Data = dto,
			Token = token
		}
		);
	}
}
