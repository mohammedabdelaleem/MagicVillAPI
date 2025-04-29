using Mapster;
using MapsterMapper;
using System.Reflection;

namespace Client_MagicVill;

public static class DependencyInjection
{

	public static IServiceCollection AddDependencies(this IServiceCollection services , IConfiguration configuration)
	{

		services.AddControllersWithViews();


		// mapster 
		var mappingConfigurations = TypeAdapterConfig.GlobalSettings;
		mappingConfigurations.Scan(Assembly.GetExecutingAssembly());
		services.AddSingleton<IMapper>(new Mapper(mappingConfigurations));


		// 
		services.AddHttpClient<IVillaService, VillaService>();
		services.AddScoped<IVillaService, VillaService>();


		return services;
	}



}
