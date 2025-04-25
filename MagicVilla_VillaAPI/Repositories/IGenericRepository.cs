using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repositories;

public interface IGenericRepository<T> where T : class
{

	// use can use get by field like id , but here i will try expression 
	Task<T?> GetAsync(Expression<Func<T, bool>> filter, bool tracking = true, CancellationToken cancellationToken = default);
	Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter=null, string include = null, CancellationToken cancellationToken = default);

	public Task<bool> IsExistsAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);
	public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

	public Task<T> AddAsync(T villa, CancellationToken cancellationToken = default);

	//public Task<bool> SaveAsync(CancellationToken cancellationToken = default); // saving from UOW


}
