
namespace MagicVilla_VillaAPI.Implementations;

public class VillaRepository : GenericRepository<Villa>, IVillaRepository
{
	private readonly AppDbContext _context;

	public VillaRepository(AppDbContext context): base(context)
	{
		_context = context;
	}

	public async Task<bool> UpdateAsync(int id ,Villa updatedVilla, CancellationToken cancellationToken=default)
	{
		

		var existingVilla = await _context.Villas.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
		if (existingVilla == null)
		{
			return false;
		}

		// Map values from updatedVilla to existingVilla, excluding immutable ones like Id and CreatedDate
		updatedVilla.Adapt(existingVilla);

		existingVilla.UpdatedDate = DateTime.Now;

		//await _context.SaveChangesAsync(cancellationToken);
		return true;
	}
}
