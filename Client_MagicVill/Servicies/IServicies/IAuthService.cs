using Client_MagicVill.Contracts.Auth;

namespace Client_MagicVill.Servicies.IServicies;

public interface IAuthService 
{
	Task<T> RegsisterAsync<T>(RegisterationRequestDTO obj);
	Task<T> LoginAsync<T>(LoginRequestDTO obj);

}
