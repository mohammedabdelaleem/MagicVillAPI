using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers.v2;
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("2.0")]

public class VillaNumberController : ControllerBase
{
	private ApiResponse _response;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ILogger<VillaNumberController> _logger;
	private readonly IVillaNumberRepository _villaNumberRepository;
	private readonly IVillaRepository _villaRepository;

	public VillaNumberController(IUnitOfWork unitOfWork,
		ILogger<VillaNumberController> logger, 
		IVillaNumberRepository villaNumberRepository,
		IVillaRepository villaRepository)
	{
		_unitOfWork = unitOfWork;
		_logger = logger;
		_villaNumberRepository = villaNumberRepository;
		_villaRepository = villaRepository;
	}



	//[MapToApiVersion("2.0")]
	[HttpGet("{villaNo:int}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public IEnumerable<string> Get(int villaNo)
	{
		return new string[] { "string1", "string2" };
	}





}