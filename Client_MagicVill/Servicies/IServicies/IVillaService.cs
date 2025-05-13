namespace Client_MagicVill.Servicies.IServicies;

public interface IVillaService
{
	Task<T> GetAllAsync<T>();
	Task<T> GetAsync<T>(int id );

	Task<T> CreateAsync<T>(VillaCreateDTO dot);
	Task<T> UpdateAsync<T>(VillaUpdateDTO dot);
	Task<T> DeleteAsync<T>(int id);


}
