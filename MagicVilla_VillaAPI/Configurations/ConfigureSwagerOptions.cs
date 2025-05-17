using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MagicVilla_VillaAPI.Configurations;

public class ConfigureSwagerOptions : IConfigureOptions<SwaggerGenOptions>
{
	private readonly IApiVersionDescriptionProvider _provider;

	public ConfigureSwagerOptions(IApiVersionDescriptionProvider provider)
	{
		_provider = provider;
	}
	public void Configure(SwaggerGenOptions options)
	{

		foreach (var desc in _provider.ApiVersionDescriptions)
		{
			options.SwaggerDoc(desc.GroupName, new OpenApiInfo
			{
				Version = desc.ApiVersion.ToString(),
				Title = $"Magic Villa {desc.ApiVersion}",
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
}
