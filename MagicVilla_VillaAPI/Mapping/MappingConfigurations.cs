using Mapster;

namespace MagicVilla_VillaAPI.Mapping;

public class MappingConfigurations : IRegister
{

public void Register(TypeAdapterConfig config)
	{
		// Protect Id & CreatedDate
		// If you want to make sure Adapt() doesn’t overwrite Id or CreatedDate

		TypeAdapterConfig<Villa, Villa>
		.NewConfig()
		.Ignore(dest => dest.Id)
		.Ignore(dest => dest.CreatedDate);



	}
}
