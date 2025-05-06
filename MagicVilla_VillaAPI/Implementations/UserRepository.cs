using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_VillaAPI.Implementations;

/// <summary>
/// This class is responsible for user-related features like checking if a username is already taken,
/// registering new users, logging in users, and generating JWT tokens.
/// </summary>
public class UserRepository : IUserRepository
{
	private readonly AppDbContext _context;
	private readonly string _secretKey;

	// Constructor: gets the database and secret key from the app settings
	public UserRepository(AppDbContext context, IConfiguration configuration)
	{
		_context = context;
		_secretKey = configuration.GetValue<string>("ApiSettings:Secret");
	}

	public async Task<LocalUser> Get(string username, CancellationToken cancellationToken = default)
	{
		return await _context.Users.FirstOrDefaultAsync(u=>u.UserName.ToLower() == username.ToLower(), cancellationToken);

	}

	/// <summary>
	/// Checks if the given username is already used by another user.
	/// Returns true if it's unique (not used).
	/// </summary>
	public async Task<bool> IsUniqueUserAsync(string username, CancellationToken cancellationToken = default)
	{
		// Look for a user in the database with the same username (case-insensitive)
		bool found = await _context.Users.AnyAsync(
			u => u.UserName.ToLower() == username.ToLower(),
			cancellationToken);

		// If not found, it's unique
		return !found;
	}

	/// <summary>
	/// Tries to log in the user by checking username and password.
	/// If valid, returns a JWT token that the client can use to stay logged in.
	/// </summary>
	public async Task<LoginResponsDTO> Login(LoginRequestDTO request, CancellationToken cancellationToken = default)
	{
		// Step 1: Try to find a user with the given username and password
		var user = await _context.Users.FirstOrDefaultAsync(
			u => u.UserName.ToLower() == request.UserName.ToLower() &&
				 u.Password == request.Password,
			cancellationToken);

		// If not found, return null (login failed)
		if (user == null)
		{
			return new LoginResponsDTO
			{
				Token = null,
				User = null
			};
		}

		// Step 2: Create a JWT token for the user

		var tokenHandler = new JwtSecurityTokenHandler();  // tool to create and handle tokens
		var key = Encoding.ASCII.GetBytes(_secretKey);     // convert our secret key to bytes

		// Token settings: what data to include, how long it's valid, and how to sign it
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(new[]
			{
				new Claim(ClaimTypes.Name, user.Id.ToString()),       // user ID as Name claim
				new Claim(ClaimTypes.Role, user.Role)      // user role (e.g., Admin/User)
			}),
			Expires = DateTime.UtcNow.AddDays(5), // token will expire in 5 days
			SigningCredentials = new SigningCredentials(
				new SymmetricSecurityKey(key),      // our secret key
				SecurityAlgorithms.HmacSha256Signature) // algorithm to sign the token
		};

		// Create the token
		var token = tokenHandler.CreateToken(tokenDescriptor);

		// Return the user info and the token as a response
		return new LoginResponsDTO
		{
			User = user,
			Token = tokenHandler.WriteToken(token)
		};
	}

	/// <summary>
	/// Registers a new user in the system.
	/// Returns the created user with password removed for safety.
	/// </summary>
	public async Task<LocalUser> Register(RegisterationRequestDTO request, CancellationToken cancellationToken = default)
	{
		// Convert the registration data into a user object (using Mapster or similar tool)
		LocalUser user = request.Adapt<LocalUser>();

		// Add the user to the database (but changes are not saved here)
		await _context.Users.AddAsync(user, cancellationToken);

		await _context.SaveChangesAsync(cancellationToken); // we need to save now , because outside password be empty , I think We Can Add AsNoTracking On Password , But let it now

		// For safety: don't return the password to the client
		user.Password = string.Empty;

		return user;
	}


}
