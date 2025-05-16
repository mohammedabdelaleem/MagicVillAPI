using MagicVilla_Utility;
using MagicVilla_VillaAPI.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
namespace MagicVilla_VillaAPI.Implementations;

public class UserRepository : IUserRepository
{
	private readonly AppDbContext _context;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly RoleManager<IdentityRole> _roleManager;
	private readonly string _secretKey;

	// Constructor: gets the database and secret key from the app settings
	public UserRepository(AppDbContext context,
		IConfiguration configuration,
		UserManager<ApplicationUser> userManager,
		RoleManager<IdentityRole> roleManager
		)
	{
		_context = context;
		_userManager = userManager;
		_roleManager = roleManager;
		_secretKey = configuration.GetValue<string>("ApiSettings:Secret");
	}

	public async Task<ApplicationUser> GetAsync(string username, CancellationToken cancellationToken = default)
	{
		return await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName.ToLower() == username.ToLower(), cancellationToken);
	}

	public async Task<bool> IsUniqueUserAsync(string username, CancellationToken cancellationToken = default)
	{
		// Look for a user in the database with the same username (case-insensitive)
		bool found = await _context.ApplicationUsers.AnyAsync(
			u => u.UserName.ToLower() == username.ToLower(),
			cancellationToken);

		// If not found, it's unique
		return !found;
	}

	public async Task<TokenDTO> LoginAsync(LoginRequestDTO request, CancellationToken cancellationToken = default)
	{
		// Step 1: Try to find a user with the given username and password
		var user = await GetAsync(request.UserName.ToLower(),
			cancellationToken);

		bool validPassword = await _userManager.CheckPasswordAsync(user, request.Password);


		// If not found, return null (login failed)
		if (user == null || !validPassword)
		{
			return new TokenDTO
			{
				AccessToken = null,
				RefreshToken = null,
			};
		}

		// Step 2: Create a JWT token for the user
		var jwtTokenId = $"JTI{Guid.NewGuid()}";
		var accessToken = await GetAccessToken(user, jwtTokenId);
		var refreshToken = await CreateNewRefreshTokenAsync(user.Id, jwtTokenId, cancellationToken);
		// Return the user info and the token as a response
		return new TokenDTO
		{
			AccessToken = accessToken,
			RefreshToken = refreshToken
		};
	}



	private async Task<string> GetAccessToken(ApplicationUser user, string jwtTokenId)
	{

		var tokenHandler = new JwtSecurityTokenHandler();  // tool to create and handle tokens
		var key = Encoding.ASCII.GetBytes(_secretKey);     // convert our secret key to bytes
		var roles = await _userManager.GetRolesAsync(user);



		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.Name, user.UserName),
			new Claim(JwtRegisteredClaimNames.Jti, jwtTokenId   ),
			new Claim(JwtRegisteredClaimNames.Sub, user.Id),
		};

		claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

		// Token settings: what data to include, how long it's valid, and how to sign it
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Expires = DateTime.UtcNow.AddMinutes(SD.AccessTokenExpiresInNMinutes), // token will expire in 5 days
			SigningCredentials = new SigningCredentials(
				new SymmetricSecurityKey(key),      // our secret key
				SecurityAlgorithms.HmacSha256Signature) // algorithm to sign the token
		};

		// Create the token
		var token = tokenHandler.CreateToken(tokenDescriptor);
		var tokenStr = tokenHandler.WriteToken(token);
		return tokenStr;
	}

	public async Task<UserDTO> RegisterAsync(RegisterationRequestDTO request, CancellationToken cancellationToken = default)
	{
		// Convert the registration data into a user object (using Mapster or similar tool)
		ApplicationUser user = request.Adapt<ApplicationUser>();
		try
		{
			var result = await _userManager.CreateAsync(user, request.Password);

			if (result.Succeeded)
			{

				// adding role here : this is for testing 
				/////////////////////
				if (!_roleManager.RoleExistsAsync(request.Role).GetAwaiter().GetResult())
				{
					await _roleManager.CreateAsync(new IdentityRole(request.Role));
				}
				/////////////////////////

				// adding role ==> But don't forget to create it first 
				await _userManager.AddToRoleAsync(user, request.Role); // hardcoded now

				var userToRetuen = await GetAsync(request.UserName.ToLower(), cancellationToken);

				return userToRetuen.Adapt<UserDTO>();

			}
			//else
			//{
			//	// logging errors
			//	var errors = result.Errors.Select(e=>e.ToString());
			//	string.Join(",",errors);
			//}
			return new UserDTO();

		}
		catch (Exception ex)
		{
			return new UserDTO();

		}

	}


	public async Task<TokenDTO> RefreshAccessToken(TokenDTO tokenDTO, CancellationToken cancellationToken = default)
	{

		// before we creating new access token 
		// validate the current one 


		// 01 - find the existing refresh token : based on the refresh token we get in the prameter
		var existingRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(u =>
		u.Refresh_Token == tokenDTO.RefreshToken);

		if (existingRefreshToken is null) return new TokenDTO();


		// 02 - Compare Data from refresh and access token provided and if there are any missmatches then consider this is a fraud {access token has userid , jti}
		var isTokenValid = GetAccessTokenData(tokenDTO.AccessToken, existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);

		if (!isTokenValid)
		{
			await MarkTokenAsInvalidAsync(existingRefreshToken, cancellationToken);
			return new TokenDTO();
		}

		// 03 - if someone tries to use invalid token , fraud possible 
		//if (!existingRefreshToken.IsValid)
		//{

		//	var chainRecords = await _context.RefreshTokens.Where(
		//		r => r.JwtTokenId == existingRefreshToken.JwtTokenId &&
		//		r.UserId == existingRefreshToken.UserId).ToListAsync(cancellationToken);

		//	foreach (var chainRecord in chainRecords)
		//	{
		//		chainRecord.IsValid= false;
		//	}

		//	_context.UpdateRange(chainRecords);
		//	await _context.SaveChangesAsync(cancellationToken);
		//	return new TokenDTO();
		//}

		// Bulk Updation 
		if (!existingRefreshToken.IsValid)
		{
			await MarkAllTokensInChainAsInvalidAsync(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId,	cancellationToken);
		}

		// 04 - If Just Expired then mark as invalid and return empty
		if (existingRefreshToken.ExpiresAt < DateTime.UtcNow)
		{
			await MarkTokenAsInvalidAsync(existingRefreshToken, cancellationToken);
			return new TokenDTO();

		}

		// 05 - Replace The Old Refresh Token with a new Refresh Token with updated expire date 
		var newRefreshToken = await CreateNewRefreshTokenAsync(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId, cancellationToken);


		// 06 - revoke the existing refresh token
		await MarkTokenAsInvalidAsync(existingRefreshToken, cancellationToken);



		// 07 - Generate New Access Token
		var applicationUser = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == existingRefreshToken.UserId);
		if (applicationUser == null)
			return new TokenDTO();

		var newAccessToken = await GetAccessToken(applicationUser, existingRefreshToken.JwtTokenId);
		//  we use the old JwtTokenId : we can easily tracked which token id(s) are related 

		return new TokenDTO()
		{
			AccessToken = newAccessToken,
			RefreshToken = newRefreshToken,
		};
	}

	public async Task RevokeRefreshTokenAsync(TokenDTO tokenDTO, CancellationToken cancellationToken = default)
	{
		var existingRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(
			t=>t.Refresh_Token == tokenDTO.RefreshToken
			,cancellationToken);

		var isTokenValid = GetAccessTokenData(tokenDTO.AccessToken, existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);

		if (!isTokenValid)
		{
			return;
		}

		await MarkAllTokensInChainAsInvalidAsync(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId, cancellationToken);
	}

	private async Task<string> CreateNewRefreshTokenAsync(string userId, string tokenId, CancellationToken cancellationToken = default)
	{
		RefreshToken refreshToken = new RefreshToken
		{
			IsValid = true,
			JwtTokenId = tokenId,
			UserId = userId,
			Refresh_Token = $"{Guid.NewGuid()}-{Guid.NewGuid()}",
			ExpiresAt = DateTime.UtcNow.AddMinutes(SD.RefreshTokenExpiresInNMinutes)
		};

		await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);

		return refreshToken.Refresh_Token;
	}


	// End point to read the access token
	private bool GetAccessTokenData(string accessToken, string expectedUserId, string expectedJwtTokenId)
	{
		try
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var jwt = tokenHandler.ReadJwtToken(accessToken);

			var userId = jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value;
			var jwtTokenId = jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Jti).Value;

			return userId == expectedUserId && jwtTokenId == expectedJwtTokenId;
		}
		catch
		{
			return false;
		}
	}

	private async Task MarkAllTokensInChainAsInvalidAsync(string userId, string tokenId, CancellationToken cancellationToken = default)
	{
		 await _context.RefreshTokens.Where(
				r => r.JwtTokenId == tokenId &&
				r.UserId == userId)
				.ExecuteUpdateAsync(u => u.SetProperty(refreshToken => refreshToken.IsValid, false),cancellationToken);

	}
	private Task MarkTokenAsInvalidAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
	{
		refreshToken.IsValid = false;
		  return _context.SaveChangesAsync(cancellationToken);
	}

	
}

