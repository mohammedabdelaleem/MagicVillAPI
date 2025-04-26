namespace MagicVilla_VillaAPI.Repositories;

public interface IUnitOfWork : IDisposable
{
	IVillaRepository Villa { get; }
	IVillaNumberRepository VillaNumber { get; }


	Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}
