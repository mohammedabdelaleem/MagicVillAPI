
using MagicVilla_VillaAPI.Contracts;

namespace MagicVilla_VillaAPI.Repositories;


// Register
// Login 
public interface IUserRepository
{
	Task<bool> IsUniqueUserAsync(string username, CancellationToken cancellationToken=default);

	Task<TokenDTO> LoginAsync (LoginRequestDTO loginRequest, CancellationToken cancellationToken = default);

	// we can return null , but now let us return New User That Has Been Created At The DB.
	Task<UserDTO> RegisterAsync(RegisterationRequestDTO registerationRequestDTO, CancellationToken cancellationToken = default);

	Task<ApplicationUser> GetAsync(string username, CancellationToken cancellationToken=default);

	Task<TokenDTO> RefreshAccessToken(TokenDTO loginRequest, CancellationToken cancellationToken = default);

}
