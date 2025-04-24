
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace MagicVilla_VillaAPI.Services;

public interface IVillaStoreService
{
	public Task<List<Villa>?> GetAllAsync(CancellationToken cancellationToken=default);
	public Task<Villa?> GetAsync(int id, bool tracking=true, CancellationToken cancellationToken= default);
	public Task<bool> IsExistsAsync(int id, CancellationToken cancellationToken = default);
	public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

	public Task<bool> EditAsync(int id, Villa villa, CancellationToken cancellationToken = default);
	public Task<Villa> AddAsync(Villa villa, CancellationToken cancellationToken = default);

}
