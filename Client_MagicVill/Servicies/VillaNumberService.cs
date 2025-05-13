using Client_MagicVill.Models;
using MagicVilla_Utility;

namespace Client_MagicVill.Servicies;

public class VillaNumberService :  IVillaNumberService
{
	private readonly IHttpClientFactory _httpClient;
	private readonly IBaseService _baseService;
	private string villaNumberApi;
	private string version;

	public VillaNumberService(IHttpClientFactory httpClient, IConfiguration configuration, ILogger<BaseService> logger, IBaseService baseService) 
	{
		_baseService = baseService;

		_httpClient = httpClient;
		villaNumberApi = configuration.GetValue<string>("UrlServices:villaApi")!;
		version = SD.CurrentApiVersion;
	}

	public async Task<T> CreateAsync<T>(VillaNumberCreateDTO dto)
	{
		return await _baseService.SendAsync<T>(
			new ApiRequest
			{
				ApiType = ApiType.POST,
				Url = villaNumberApi + $"/api/{version}/villaNumber",
				Data = dto,
			}
			);
	}
	public async Task<T> DeleteAsync<T>(int id )
	{
		return await _baseService.SendAsync<T>(
		new ApiRequest
		{
			ApiType = ApiType.DELETE,
			Url = villaNumberApi + $"/api/{version}/villaNumber/"+id
		}
		);
	}
	public async Task<T> GetAllAsync<T>()
	{
			return await _baseService.SendAsync<T>(
			new ApiRequest
			{
				ApiType = ApiType.GET,
				Url = villaNumberApi + $"/api/{version}/villaNumber/all"
			}
			);
	}
	public async Task<T> GetAsync<T>(int id )
	{
		return await _baseService.SendAsync<T>(
		new ApiRequest
		{
			ApiType = ApiType.GET,
			Url = villaNumberApi + $"/api/{version}/villaNumber/" +id
		}
		);
	}
	public async Task<T> UpdateAsync<T>(VillaNumberUpdateDTO dto )
	{
		return await _baseService.SendAsync<T>(
		new ApiRequest
		{
			ApiType = ApiType.PUT,
			Url = villaNumberApi + $"/api/{version}/villaNumber/" +dto.VillaNo,
			Data = dto,
		}
		);
	}
}
