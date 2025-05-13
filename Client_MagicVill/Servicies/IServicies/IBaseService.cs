using Client_MagicVill.Models;

namespace Client_MagicVill.Servicies.IServicies;

public interface IBaseService
{
	ApiResponse _responseModel { get; set; }

	Task<T> SendAsync<T>(ApiRequest apiRequest, bool withBearer = true);
}
