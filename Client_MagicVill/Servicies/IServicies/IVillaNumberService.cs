namespace Client_MagicVill.Servicies.IServicies;

public interface IVillaNumberService
{

	Task<T> GetAllAsync<T>();
	Task<T> GetAsync<T>(int id);
	Task<T> CreateAsync<T>(VillaNumberCreateDTO dot );
	Task<T> UpdateAsync<T>(VillaNumberUpdateDTO dot );
	Task<T> DeleteAsync<T>(int id);

}
