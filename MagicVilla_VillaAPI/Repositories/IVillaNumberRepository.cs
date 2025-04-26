namespace MagicVilla_VillaAPI.Repositories;

public interface IVillaNumberRepository : IGenericRepository<VillaNumber> 
{
	Task<bool> Update(int id, VillaNumber villa, CancellationToken cancellationToken = default);
}