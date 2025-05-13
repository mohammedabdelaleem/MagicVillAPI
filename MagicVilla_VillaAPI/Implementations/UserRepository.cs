using MagicVilla_VillaAPI.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
		return await _context.ApplicationUsers.FirstOrDefaultAsync(u=>u.UserName.ToLower() == username.ToLower(), cancellationToken);
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

	public async Task<TokenDTO> Login(LoginRequestDTO request, CancellationToken cancellationToken = default)
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
			};
		}

		// Step 2: Create a JWT token for the user
		var accessToken = await GetAccessToken(user);

		// Return the user info and the token as a response
		return new TokenDTO
		{

			AccessToken = accessToken
		};
	}



	private async Task<string> GetAccessToken(ApplicationUser user)
	{

		var tokenHandler = new JwtSecurityTokenHandler();  // tool to create and handle tokens
		var key = Encoding.ASCII.GetBytes(_secretKey);     // convert our secret key to bytes
		var roles = await _userManager.GetRolesAsync(user);



		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.NameIdentifier, user.Id),       // user ID as Name claim
			new Claim(ClaimTypes.Name, user.UserName)
		};

		claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

		// Token settings: what data to include, how long it's valid, and how to sign it
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Expires = DateTime.UtcNow.AddDays(5), // token will expire in 5 days
			SigningCredentials = new SigningCredentials(
				new SymmetricSecurityKey(key),      // our secret key
				SecurityAlgorithms.HmacSha256Signature) // algorithm to sign the token
		};

		// Create the token
		var token = tokenHandler.CreateToken(tokenDescriptor);
		var tokenStr = tokenHandler.WriteToken(token);
		return tokenStr;
	}

	public async Task<UserDTO> Register(RegisterationRequestDTO request, CancellationToken cancellationToken = default)
	{
		// Convert the registration data into a user object (using Mapster or similar tool)
		ApplicationUser user = request.Adapt<ApplicationUser>();
		try
		{
			var result = await _userManager.CreateAsync(user,request.Password);

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
		}
		catch (Exception ex)
		{

		}

		return new UserDTO();
	}


}
