using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
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


		// adding service life time
		services.AddScoped<IUnitOfWork, UnitOfWork>();
		services.AddScoped<IVillaRepository, VillaRepository>();
		services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();
		services.AddScoped<IUserRepository, UserRepository>();





		// Add Authentication

		var key = configuration.GetValue<string>("ApiSettings:Secret");

		services.AddAuthentication(x =>
		{
			x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
			.AddJwtBearer(x => {
				x.RequireHttpsMetadata = false;
				x.SaveToken = true;
				x.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
					ValidateIssuer = false,
					ValidateAudience = false
				};
			}); ;


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
