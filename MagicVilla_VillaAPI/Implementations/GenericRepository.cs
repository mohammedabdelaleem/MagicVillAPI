using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Implementations;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
	private readonly AppDbContext _context;
	private DbSet<T> _dbSet;

	public GenericRepository(AppDbContext context)
	{
		_context = context;
		_dbSet = _context.Set<T>();
	}

	public async Task<T> AddAsync(T model, CancellationToken cancellationToken = default)
	{
			await _dbSet.AddAsync(model, cancellationToken);
			//await SaveAsync(cancellationToken);
			return model; 		
	}

	public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
	{
		var model = await _dbSet.FindAsync(id, cancellationToken);
		if (model == null)
		{
			return false;
		}

		_dbSet.Remove(model!);
		//await SaveAsync(cancellationToken);
		return true;
	}

	public async Task<List<T>?> GetAllAsync(Expression<Func<T, bool>>? filter = null, string include = "",CancellationToken cancellationToken = default)
	{
		IQueryable<T> query = _dbSet.AsQueryable();

		if (filter != null)
			query = query.Where(filter);

		if (include != null && !string.IsNullOrWhiteSpace(include))
		{
			// context.Categories.Include("Users,Logos,Products").Tolist();
			foreach (var entity in include.Split(",", StringSplitOptions.RemoveEmptyEntries))
			{
				query = query.Include(entity);
			}
		}
		return await query.ToListAsync(cancellationToken);
	}

	public async Task<T?> GetAsync(Expression<Func<T, bool>> filter, bool tracking =true, string include = null, CancellationToken cancellationToken = default)
	{

		IQueryable<T> query = _dbSet;


		if (!string.IsNullOrWhiteSpace(include))
		{
			foreach (var entity in include.Split(",", StringSplitOptions.RemoveEmptyEntries))
			{
				query = query.Include(entity.Trim());
			}
		}

		if (!tracking)
			query = query.AsNoTracking();

		return await query.FirstOrDefaultAsync(filter, cancellationToken);
	}

	public async Task<bool> IsExistsAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
	{
		bool isExists = await _dbSet.AnyAsync(filter , cancellationToken);
		return isExists;
	}

}
