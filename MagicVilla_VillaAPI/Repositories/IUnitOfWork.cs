namespace MagicVilla_VillaAPI.Repositories;

public interface IUnitOfWork : IDisposable
{
	IVillaRepository Villa { get; }

	Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}
