namespace Client_MagicVill.Servicies.IServicies;

public interface IVillaNumberService
{

	Task<T> GetAllAsync<T>(string token);
	Task<T> GetAsync<T>(int id, string token);
	Task<T> CreateAsync<T>(VillaNumberCreateDTO dot, string token);
	Task<T> UpdateAsync<T>(VillaNumberUpdateDTO dot, string token);
	Task<T> DeleteAsync<T>(int id, string token);

}
