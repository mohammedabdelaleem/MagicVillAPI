namespace MagicVilla_VillaAPI;

public static class DependencyInjection
{

	public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
	{

		services.AddControllers(option => {
			option.ReturnHttpNotAcceptable = true;

		}).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();

		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen();



		return services;
	}


}
