namespace MagicVilla_VillaAPI.Repositories;

public interface IVillaRepository : IGenericRepository<Villa>
{
	Task<bool> Update(int id, Villa villa, CancellationToken cancellationToken = default);
}

