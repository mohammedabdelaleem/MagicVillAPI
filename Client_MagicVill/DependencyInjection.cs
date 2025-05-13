using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
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
		 
		services.AddHttpClient<IVillaNumberService, VillaNumberService>();
		services.AddScoped<IVillaNumberService, VillaNumberService>();


		services.AddHttpClient<IAuthService, AuthService>();
		services.AddScoped<IAuthService, AuthService>();

		services.AddScoped<IBaseService, BaseService>();

		// session 
		services.AddSingleton
			<IHttpContextAccessor, HttpContextAccessor>();

		services.AddScoped<ITokenProvider, TokenProvider>();


		services.AddDistributedMemoryCache();
		services.AddSession(options =>
		{
			options.IdleTimeout = TimeSpan.FromMinutes(100);
			options.Cookie.HttpOnly = true;
			options.Cookie.IsEssential = true;
		});


		// default Authentication Scheme
		services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
			.AddCookie(options =>
			{
				options.Cookie.HttpOnly = true;
				options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
				options.SlidingExpiration = true;
				options.LoginPath = "/Auth/Login";
				options.AccessDeniedPath = "/Auth/AccessDenied";
			});

		return services;
	}



}
