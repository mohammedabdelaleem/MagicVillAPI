using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace MagicVilla_VillaAPI;

public static class DependencyInjection
{

	public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
	{
		//CacheProfile  ==> DRY
		services.AddControllers(options =>
		{
			options.CacheProfiles.Add("Default30",
				new CacheProfile()
				{
					Duration = 30
				});

			options.CacheProfiles.Add("NoCache",
				new CacheProfile()
				{
					Location = ResponseCacheLocation.None,
					NoStore = true
				});
		})
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
	})
	.AddNewtonsoftJson(); // Required for JsonPatchDocument



		services
			.AddSwaggerConfig()
			.AddDatabaseConfig(configuration)
			.AddMapsterConfig()
			.AddResponseCaching(); // cashing 


		// adding service life time
		services.AddScoped<IUnitOfWork, UnitOfWork>();
		services.AddScoped<IVillaRepository, VillaRepository>();
		services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();
		services.AddScoped<IUserRepository, UserRepository>();



		// Identity
		services.AddIdentity<ApplicationUser, IdentityRole>()
			.AddEntityFrameworkStores<AppDbContext>();




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


		// swager and versioning cofig
		services.AddSwaggerGen(options =>
		{
			options.SwaggerDoc("v1", new OpenApiInfo
			{
				Version = "v1.0",
				Title = "Magic Villa V1",
				Description = "API to manage Villa",
				TermsOfService = new Uri("https://example.com/terms"),
				Contact = new OpenApiContact
				{
					Name = "Dotnetmastery",
					Url = new Uri("https://dotnetmastery.com")
				},
				License = new OpenApiLicense
				{
					Name = "Example License",
					Url = new Uri("https://example.com/license")
				}
			});
			options.SwaggerDoc("v2", new OpenApiInfo
			{
				Version = "v2.0",
				Title = "Magic Villa V2",
				Description = "API to manage Villa",
				TermsOfService = new Uri("https://example.com/terms"),
				Contact = new OpenApiContact
				{
					Name = "Dotnetmastery",
					Url = new Uri("https://dotnetmastery.com")
				},
				License = new OpenApiLicense
				{
					Name = "Example License",
					Url = new Uri("https://example.com/license")
				}
			});
		});
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
