namespace MagicVilla_VillaAPI.Repositories;

public interface IUnitOfWork : IDisposable
{
	IVillaRepository Villa { get; }
	IVillaNumberRepository VillaNumber { get; }

	IUserRepository User { get; }

	Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}
