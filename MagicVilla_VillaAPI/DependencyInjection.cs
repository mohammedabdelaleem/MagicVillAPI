using System.Reflection;
using System.Text.Json;

namespace MagicVilla_VillaAPI;

public static class DependencyInjection
{

	public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
	{	

		services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
	})
	.AddNewtonsoftJson(); // Required for JsonPatchDocument



		services
			.AddSwaggerConfig()
			.AddDatabaseConfig(configuration)
			.AddMapsterConfig();

		

		services.AddScoped<IUnitOfWork, UnitOfWork>();
		services.AddScoped<IVillaRepository, VillaRepository>();


		return services;
	}


	private static IServiceCollection AddDatabaseConfig(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<AppDbContext>(options =>
		{
			options.UseSqlServer(configuration.GetConnectionString("constr"));
		});

	return services;
	}


	private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
	{
		// Register Mapster 
		var mappingConfigurations = TypeAdapterConfig.GlobalSettings;
		mappingConfigurations.Scan(Assembly.GetExecutingAssembly());
		services.AddScoped<IMapper>(provider => new Mapper(TypeAdapterConfig.GlobalSettings));

		return services;
	}

	private static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
	{
		// Swagger service
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen();

		return services;
	}



}
