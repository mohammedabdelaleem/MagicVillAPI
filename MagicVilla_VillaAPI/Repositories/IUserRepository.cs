
namespace MagicVilla_VillaAPI.Repositories;


// Register
// Login 
public interface IUserRepository
{
	Task<bool> IsUniqueUserAsync(string username, CancellationToken cancellationToken=default);

	Task<LoginResponsDTO> Login (LoginRequestDTO loginRequest, CancellationToken cancellationToken = default);

	// we can return null , but now let us return New User That Has Been Created At The DB.
	Task<LocalUser> Register(RegisterationRequestDTO registerationRequestDTO, CancellationToken cancellationToken = default);

	Task<LocalUser> Get(string username, CancellationToken cancellationToken=default);
}
