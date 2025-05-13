using MagicVilla_Utility;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Client_MagicVill.Controllers;
public class VillaController : Controller
{
	private readonly IVillaService _villaService;
	public VillaController(IVillaService villaService)
	{
		_villaService = villaService;
	}


	//[Authorize(Roles = "admin,user")]
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


	[Authorize(Roles = "admin")]
	public async Task<IActionResult> Create()
	{
	  return  View();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Roles = "admin")]
	public async Task<IActionResult> Create(VillaCreateDTO model)
	{

		if (ModelState.IsValid)
		{
			var response = await _villaService.CreateAsync<ApiResponse>(model );

			if (response != null && response.IsSuccess)
			{
				TempData["success"] = "Villa Created Successfully";
				return RedirectToAction(nameof(Index));
			}
		}

		TempData["error"] = "error encountered";
		return View(model);
	}


	[Authorize(Roles = "admin")]
	public async Task<IActionResult> Update(int villaId)
	{

		var response = await _villaService.GetAsync<ApiResponse>(villaId );

		if (response != null && response.IsSuccess)
		{
			VillaDTO model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result));
			return View(model.Adapt<VillaUpdateDTO>());

		}

		return NotFound();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Roles = "admin")]
	public async Task<IActionResult> Update(VillaUpdateDTO model)
	{

		if (ModelState.IsValid)
		{
			var response = await _villaService.UpdateAsync<ApiResponse>(model );

			if (response != null && response.IsSuccess)
			{
				TempData["success"] = "Villa Updated Successfully";
				return RedirectToAction(nameof(Index));
			}
		}

		TempData["error"] = "error encountered";
		return View(model);

	}



	[Authorize(Roles = "admin")]
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
	[Authorize(Roles = "admin")]
	public async Task<IActionResult> Delete(VillaDTO model)
	{


		var response = await _villaService.DeleteAsync<ApiResponse>(model.Id );

		if (response != null && response.IsSuccess)
		{
			TempData["success"] = "Villa Deleted Successfully";
			return RedirectToAction(nameof(Index));
		}

		TempData["error"] = "error encountered";
		return View(model);

	}
}
