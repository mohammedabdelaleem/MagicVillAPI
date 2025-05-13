using Client_MagicVill.Models;
using Client_MagicVill.Servicies.IServicies;
using MagicVilla_Utility;

namespace Client_MagicVill.Servicies;

public class VillaService : IVillaService
{
	private readonly IHttpClientFactory _httpClient;
	private readonly IBaseService _baseService;
	private string villaApi;
	private string version;

	public VillaService(IHttpClientFactory httpClient, IConfiguration configuration, ILogger<BaseService> logger, IBaseService baseService)
		{
		_httpClient = httpClient;
		_baseService = baseService;
		villaApi = configuration.GetValue<string>("UrlServices:VillaApi")!;
		version = SD.CurrentApiVersion;
	}

	public async Task<T> CreateAsync<T>(VillaCreateDTO dto )
	{
		return await _baseService.SendAsync<T>(
			new ApiRequest
			{
				ApiType = ApiType.POST,
				Url = villaApi + $"/api/{version}/VillaAPI",
				Data = dto,
				ContentType = ContentType.MultipartFormData,
			}
			);
	}

	public async Task<T> DeleteAsync<T>(int id)
	{
		return await _baseService.SendAsync<T>(
		new ApiRequest
		{
			ApiType = ApiType.DELETE,
			Url = villaApi + $"/api/{version}/VillaAPI/" + id
		}
		);
	}

	public async Task<T> GetAllAsync<T>()
	{
			return await _baseService.SendAsync<T>(
			new ApiRequest
			{
				ApiType = ApiType.GET,
				Url = villaApi + $"/api/{version}/VillaAPI/all"
			}
			);
	}

	public async Task<T> GetAsync<T>(int id )
	{
		return await _baseService.SendAsync<T>(
		new ApiRequest
		{
			ApiType = ApiType.GET,
			Url = villaApi + $"/api/{version}/VillaAPI/" + id,
			
		}
		);
	}

	public async Task<T> UpdateAsync<T>(VillaUpdateDTO dto )
	{
		return await _baseService.SendAsync<T>(
		new ApiRequest
		{
			ApiType = ApiType.PUT,
			Url = villaApi + $"/api/{version}/VillaAPI/" + dto.Id,
			Data = dto,
			ContentType = ContentType.MultipartFormData,
		}
		);
	}
}
