using System.Threading.Tasks;

namespace MagicVilla_VillaAPI.Implementations;

public class UnitOfWork : IUnitOfWork
{
	private readonly AppDbContext _context;
	public IVillaRepository Villa { get; private set; }
	public IVillaNumberRepository VillaNumber { get; private set; }


	public UnitOfWork(AppDbContext context)
	{
		_context = context;
		Villa = new VillaRepository(context); ///////////////
		VillaNumber = new VillaNumberRepository(context); ///////////////

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
