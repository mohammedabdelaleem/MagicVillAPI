using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace MagicVilla_VillaAPI.Implementations;

public class UnitOfWork : IUnitOfWork
{
	private readonly AppDbContext _context;
	private readonly IConfiguration _configuration;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly RoleManager<IdentityRole> _roleManager;

	public IVillaRepository Villa { get; private set; }
	public IVillaNumberRepository VillaNumber { get; private set; }
	public IUserRepository User { get; private set; }


	public UnitOfWork(AppDbContext context, IConfiguration configuration,
		UserManager<ApplicationUser> userManager,
		RoleManager<IdentityRole> roleManager
		)
	{
		_context = context;
		_configuration = configuration;
		_userManager = userManager;
		_roleManager = roleManager;

		Villa = new VillaRepository(context); ///////////////
		VillaNumber = new VillaNumberRepository(context); ///////////////
		User = new UserRepository(context, _configuration,_userManager, _roleManager);
	}
	public async Task<int> CompleteAsync(CancellationToken cancellationToken=default)
	{
		return await _context.SaveChangesAsync(cancellationToken);
	}

	public void Dispose()
	{
		_context.Dispose();
	}

	
}
