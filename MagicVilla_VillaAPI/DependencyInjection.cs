using MagicVilla_VillaAPI.Configurations;
using MagicVilla_VillaAPI.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace MagicVilla_VillaAPI;

public static class DependencyInjection
{
	public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
	{
		// Controllers with cache profiles and JSON options
		services.AddControllers(options =>
		{
			options.CacheProfiles.Add("Default30", new CacheProfile { Duration = 30 });
			options.CacheProfiles.Add("NoCache", new CacheProfile
			{
				Location = ResponseCacheLocation.None,
				NoStore = true
			});

			options.Filters.Add<CustomeExceptionFilter>();
		})
		.AddJsonOptions(options =>
		{
			options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
		})
		.AddNewtonsoftJson()// For JsonPatchDocument support
		.ConfigureApiBehaviorOptions(options =>
		{
			options.ClientErrorMapping[StatusCodes.Status500InternalServerError] = new ClientErrorData
			{
				Link = "https://neetcode.io/roadmap" // if the server returns a custome error endpoint you can defined what status code for this endpoint and provide a link for that 

			};
			options.ClientErrorMapping[StatusCodes.Status501NotImplemented] = new ClientErrorData
			{
				Link = "https://www.udemy.com/"
			};
		});

		// Add Swagger, DB, Mapping, and Response Caching
		services
			.AddSwaggerConfig()
			.AddDatabaseConfig(configuration)
			.AddMapsterConfig()
			.AddResponseCaching();

		// Scoped services
		services.AddScoped<IUnitOfWork, UnitOfWork>();
		services.AddScoped<IVillaRepository, VillaRepository>();
		services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();
		services.AddScoped<IUserRepository, UserRepository>();

		// Identity setup
		services.AddIdentity<ApplicationUser, IdentityRole>()
			.AddEntityFrameworkStores<AppDbContext>();

		// Authentication config
		services.AddAuthConfig(configuration);

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
		var mappingConfigurations = TypeAdapterConfig.GlobalSettings;
		mappingConfigurations.Scan(Assembly.GetExecutingAssembly());

		services.AddScoped<IMapper>(provider => new Mapper(TypeAdapterConfig.GlobalSettings));

		return services;
	}

	private static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
	{

		services.AddEndpointsApiExplorer();
		services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwagerOptions>();	
		services.AddSwaggerGen();

		services.AddApiVersioning(options =>
		{
			options.AssumeDefaultVersionWhenUnspecified = true;
			options.DefaultApiVersion = new ApiVersion(1, 0);
			options.ReportApiVersions = true;
		});

		services.AddVersionedApiExplorer(options =>
		{
			options.GroupNameFormat = "'v'VVV";
			options.SubstituteApiVersionInUrl = true;
		});

		return services;
	}

	private static IServiceCollection AddAuthConfig(this IServiceCollection services, IConfiguration configuration)
	{
		var key = configuration.GetValue<string>("ApiSettings:Secret");

		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(options =>
		{
			options.RequireHttpsMetadata = false;
			options.SaveToken = true;
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
				ValidIssuer = configuration.GetValue<string>("Jwt:Issuer"),
				ValidAudience = configuration.GetValue<string>("Jwt:Audience"),
				ValidateIssuer = true,
				ValidateAudience = true,
				ClockSkew = TimeSpan.Zero // No tolerance for expired tokens
			};
		});

		return services;
	}
}
