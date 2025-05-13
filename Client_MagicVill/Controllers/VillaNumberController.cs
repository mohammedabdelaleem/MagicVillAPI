using MagicVilla_Utility;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Client_MagicVill.Controllers;
public class VillaNumberController : Controller
{
	private readonly IVillaNumberService _villaNumberService;
	private readonly IVillaService _villaService;

	public VillaNumberController(IVillaNumberService villaNumberService, IVillaService villaService)
	{
		_villaNumberService = villaNumberService;
		_villaService = villaService;
	}



	public async Task<IActionResult> Index()
	{
		List<VillaNumberDTO> villas = new();

		// Api Will Always Return The Type : APIResponse
		var response = await _villaNumberService.GetAllAsync<ApiResponse>();

		if (response != null && response.IsSuccess)
		{
			villas = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(response.Result));
		}

		return View(villas);
	}

	[Authorize(Roles = "admin")]
	public async Task<IActionResult> Create()
	{
		var model = new VillaNumberCreateDTOWithItsVillaNameVM();


		// Api Will Always Return The Type : APIResponse
		var response = await _villaService.GetAllAsync<ApiResponse>();

		if (response != null && response.IsSuccess)
		{
			model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
				(Convert.ToString(response.Result)).Select(x => new SelectListItem
				{
					Text = x.Name,
					Value = x.Id.ToString()
				});
		}


		return View(model);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Roles = "admin")]
	public async Task<IActionResult> Create([FromForm]VillaNumberCreateDTOWithItsVillaNameVM model)
	{

		if (ModelState.IsValid)
		{
			var response = await _villaNumberService.CreateAsync<ApiResponse>(model.VillaNumber);

			if (response != null && response.IsSuccess)
			{
				TempData["success"] = "New Villa Number Created Successfully";
				return RedirectToAction(nameof(Index));
			}
			else
			{
				if(response.ErrorMessages.Count > 0)
				{
					TempData["error"] = "error encountered";
					ModelState.AddModelError("ErrorMessages" , response.ErrorMessages.FirstOrDefault());	
				}
			}

			
		}


		var ApiResponse = await _villaService.GetAllAsync<ApiResponse>();
		model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
		(Convert.ToString(ApiResponse.Result)).Select(x => new SelectListItem
		{
			Text = x.Name,
			Value = x.Id.ToString()
		});

		TempData["error"] = "error encountered";
		return View(model);
	}


	[Authorize(Roles = "admin")]
	public async Task<IActionResult> Update(int villaNo)
	{
		var model = new VillaNumberUpdateDTOWithItsVillaNameVM();

		var response = await _villaNumberService.GetAsync<ApiResponse>(villaNo);
		if (response != null && response.IsSuccess)
		{
			VillaNumberDTO villaNumber = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
			model.VillaNumber = villaNumber.Adapt<VillaNumberUpdateDTO>();
		}

		response = await _villaService.GetAllAsync<ApiResponse>();
		if (response != null && response.IsSuccess)
		{
			model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
				(Convert.ToString(response.Result)).Select(x => new SelectListItem
				{
					Text = x.Name,
					Value = x.Id.ToString()
				});


			return View(model);
		}

		return NotFound();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Roles = "admin")]
	public async Task<IActionResult> Update(VillaNumberUpdateDTOWithItsVillaNameVM model)
	{
		if (ModelState.IsValid)
		{
			var response = await _villaNumberService.UpdateAsync<ApiResponse>(model.VillaNumber );

			if (response != null && response.IsSuccess)
			{
				TempData["success"] = "Villa Number Updated Successfully";
				return RedirectToAction(nameof(Index));
			}
			else
			{
				if (response.ErrorMessages.Count > 0)
				{
					TempData["error"] = "error encountered";
					ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault());
				}
			}
		}


		var ApiResponse = await _villaService.GetAllAsync<ApiResponse>( );
		model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
		(Convert.ToString(ApiResponse.Result)).Select(x => new SelectListItem
		{
			Text = x.Name,
			Value = x.Id.ToString()
		});

		TempData["error"] = "error encountered";
		return View(model);
	}


	[Authorize(Roles = "admin")]
	public async Task<IActionResult> Delete(int villaNo)
	{

		var model = new VillaNumberDeleteDTOWithItsVillaNameVM();

		var response = await _villaNumberService.GetAsync<ApiResponse>(villaNo
			);
		if (response != null && response.IsSuccess)
		{
			model.VillaNumber = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
		}

		response = await _villaService.GetAllAsync<ApiResponse>();
		if (response != null && response.IsSuccess)
		{
			model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
				(Convert.ToString(response.Result)).Select(x => new SelectListItem
				{
					Text = x.Name,
					Value = x.Id.ToString()
				});

			//TempData[key: "error"] = "error encountered";
			return View(model);
		}

		TempData["error"] = "Not Found";
		return NotFound();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	[Authorize(Roles = "admin")]
	public async Task<IActionResult> Delete(VillaNumberDeleteDTOWithItsVillaNameVM model)
	{


		var response = await _villaNumberService.DeleteAsync<ApiResponse>(model.VillaNumber.VillaNo );

		if (response != null && response.IsSuccess)
		{
			TempData["success"] = "Villa Number Deleted Successfully";
			return RedirectToAction(nameof(Index));
		}

		TempData["error"] = "error encountered";
		return View(model);

	}


}


