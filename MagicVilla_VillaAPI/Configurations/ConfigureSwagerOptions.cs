using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MagicVilla_VillaAPI.Configurations;

public class ConfigureSwagerOptions : IConfigureOptions<SwaggerGenOptions>
{
	public void Configure(SwaggerGenOptions options)
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


	}
}
