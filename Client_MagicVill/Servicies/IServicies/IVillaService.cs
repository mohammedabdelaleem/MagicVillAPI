namespace Client_MagicVill.Servicies.IServicies;

public interface IVillaService
{
	Task<T> GetAllAsync<T>(string token);
	Task<T> GetAsync<T>(int id , string token);

	Task<T> CreateAsync<T>(VillaCreateDTO dot, string token);
	Task<T> UpdateAsync<T>(VillaUpdateDTO dot, string token);
	Task<T> DeleteAsync<T>(int id, string token);


}
