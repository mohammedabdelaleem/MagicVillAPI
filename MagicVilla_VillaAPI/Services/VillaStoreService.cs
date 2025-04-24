
using MagicVilla_VillaAPI.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Services;

public class VillaStoreService : IVillaStoreService
{
	private readonly AppDbContext _context;

	public VillaStoreService(AppDbContext context)
	{
		_context = context;
	}

	public async Task<Villa> AddAsync(Villa villa, CancellationToken cancellationToken = default)
	{
		try
		{
			await _context.Villas.AddAsync(villa, cancellationToken);
			await _context.SaveChangesAsync(cancellationToken);
			return villa;
		}
		catch 
		{
			return null;
		}
	
	}

	public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
	{
		var villa = await _context.Villas.FindAsync(id, cancellationToken);
		if (villa == null)
		{
			return false;
		}

		_context.Villas.Remove(villa!);
		await _context.SaveChangesAsync(cancellationToken);
		return true;
	}

	public async Task<bool> EditAsync(int id, Villa updatedVilla, CancellationToken cancellationToken = default)
	{
		try
		{
			var existingVilla = await _context.Villas.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
			if (existingVilla == null)
			{
				return false;
			}

			// Map values from updatedVilla to existingVilla, excluding immutable ones like Id and CreatedDate
			updatedVilla.Adapt(existingVilla);

			existingVilla.UpdatedDate = DateTime.Now;

			await _context.SaveChangesAsync(cancellationToken);
			return true;
		}
		catch
		{
			return false;
		}
	}

	public async Task<List<Villa>?> GetAllAsync(CancellationToken cancellationToken = default)
	{
		var villas = await _context.Villas.ToListAsync(cancellationToken);
		return villas;
	}

	public async Task<Villa?> GetAsync(int id, bool tracking , CancellationToken cancellationToken = default)
	{
		Villa villa;
		if (tracking)
			villa = await _context.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

		else
		 villa = await _context.Villas.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

		return villa;
	}
	public async Task<bool> IsExistsAsync(int id, CancellationToken cancellationToken = default)
	{
		bool isExists = await _context.Villas.AnyAsync(v=>v.Id == id, cancellationToken);
		return isExists;
	}
}
