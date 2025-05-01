using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Client_MagicVill.Controllers;
public class HomeController : Controller
{
	private readonly IVillaService _villaService;

	public HomeController(IVillaService villaService)
	{
		_villaService = villaService;
	}

	public async Task<IActionResult> Index()
	{
		List<VillaDTO> villas = new();

		// Api Will Always Return The Type : APIResponse
		var response = await _villaService.GetAllAsync<ApiResponse>();

		if (response != null && response.IsSuccess)
		{
			villas = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));
		}

		return View(villas);
	}

}
