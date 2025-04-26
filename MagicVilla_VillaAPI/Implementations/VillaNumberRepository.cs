using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Implementations;

public class VillaNumberRepository : GenericRepository<VillaNumber>, IVillaNumberRepository
{
	private readonly AppDbContext _context;

	public VillaNumberRepository(AppDbContext context) : base(context)
	{
		_context = context;
	}

	public async Task<bool> Update(int villaNo, VillaNumber updatedVillaNumber, CancellationToken cancellationToken = default)
	{

		var villaNumber = await _context.VillaNumbers.FirstOrDefaultAsync(v => v.VillaNo == villaNo, cancellationToken);
		if(villaNumber == null) 
			return false;

		updatedVillaNumber.Adapt(villaNumber);
		villaNumber.UpdatedAt = DateTime.UtcNow;

		return true;
	}
}
