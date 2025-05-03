using System.Threading.Tasks;

namespace MagicVilla_VillaAPI.Implementations;

public class UnitOfWork : IUnitOfWork
{
	private readonly AppDbContext _context;
	private readonly IConfiguration _configuration;
	public IVillaRepository Villa { get; private set; }
	public IVillaNumberRepository VillaNumber { get; private set; }
	public IUserRepository User { get; private set; }


	public UnitOfWork(AppDbContext context, IConfiguration configuration)
	{
		_context = context;
		_configuration = configuration;

		Villa = new VillaRepository(context); ///////////////
		VillaNumber = new VillaNumberRepository(context); ///////////////
		User = new UserRepository(context, _configuration);
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
