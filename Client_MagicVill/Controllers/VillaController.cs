using Client_MagicVill.Contracts.Villa;
using Client_MagicVill.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Client_MagicVill.Controllers;
public class VillaController : Controller
{
	private readonly IVillaService _villaService;

	public VillaController(IVillaService villaService)
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

	public async Task<IActionResult> Create()
	{
		return View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(VillaCreateDTO model)
	{

		if (ModelState.IsValid)
		{
			var response = await _villaService.CreateAsync<ApiResponse>(model);

			if (response != null && response.IsSuccess)
			{
				return RedirectToAction(nameof(Index));
			}
		}

		return View(model);
	}


	public async Task<IActionResult> Update(int villaId)
	{

		var response = await _villaService.GetAsync<ApiResponse>(villaId);

		if (response != null && response.IsSuccess)
		{
			VillaDTO model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result));
			return View(model.Adapt<VillaUpdateDTO>());

		}

		return NotFound();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Update(VillaUpdateDTO model)
	{

		if (ModelState.IsValid)
		{
			var response = await _villaService.UpdateAsync<ApiResponse>(model);

			if (response != null && response.IsSuccess)
			{
				return RedirectToAction(nameof(Index));
			}
		}

		return View(model);

	}




	public async Task<IActionResult> Delete(int villaId)
	{

		var response = await _villaService.GetAsync<ApiResponse>(villaId);

		if (response != null && response.IsSuccess)
		{
			VillaDTO model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result));
			return View(model);

		}

		return NotFound();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Delete(VillaDTO model)
	{


		var response = await _villaService.DeleteAsync<ApiResponse>(model.Id);

		if (response != null && response.IsSuccess)
		{
			return RedirectToAction(nameof(Index));
		}


		return View(model);

	}


}
