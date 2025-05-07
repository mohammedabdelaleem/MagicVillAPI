using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers.v1;
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]

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

	[HttpGet("all")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<ActionResult<ApiResponse>> GetAll(CancellationToken cancellation = default)
	{
		try
		{
			var villas = await _unitOfWork.VillaNumber.GetAllAsync(include: nameof(Villa), cancellationToken: cancellation);

			_response = new(statusCode: HttpStatusCode.OK, result: villas);

			return Ok(_response);
		}
		catch (Exception ex)
		{
			_response = new(statusCode: HttpStatusCode.InternalServerError, isSuccess: false, errorMessages: new List<string>() { ex.ToString() });
			return _response;
		}
	}





	//[MapToApiVersion("1.0")]
	[HttpGet("{villaNo:int}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<ApiResponse>> Get(int villaNo, CancellationToken cancellationToken = default)
	{
		try
		{
			if (villaNo <= 0)
			{           //return BadRequest(new { message = "Invalid ID. ID must be greater than zero." });

				_response = new(statusCode: HttpStatusCode.BadRequest, isSuccess: false);
				return BadRequest(_response);
			}

			var villa = await _unitOfWork.VillaNumber.GetAsync(v => v.VillaNo == villaNo, include: nameof(Villa), cancellationToken: cancellationToken);

			if (villa == null)
			{
				//return NotFound(new { message = $"Villa with ID {villaNo} not found." });
				_response = new(statusCode: HttpStatusCode.NotFound, isSuccess: false);
				return NotFound(_response);
			}


			_response = new(statusCode: HttpStatusCode.OK, result: villa);

			return Ok(_response);
		}
		catch (Exception ex)
		{
			_response = new(statusCode: HttpStatusCode.InternalServerError, isSuccess: false, errorMessages: new List<string>() { ex.ToString() });
			return _response;
		}
	}


	[HttpPost]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[Authorize(Roles = "admin")]
	public async Task<ActionResult<ApiResponse>> Add([FromBody] VillaNumberCreateDTO villaNumberRequest, CancellationToken cancellationToken = default)
	{
		try
		{
			if (await _villaNumberRepository.IsExistsAsync(v => v.VillaNo == villaNumberRequest.VillaNo))
			{
				ModelState.AddModelError("ErrorMessages", "Vill Number Already Found");
				return BadRequest(ModelState);
			}



			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var createdVilla = await _unitOfWork.VillaNumber.AddAsync(villaNumberRequest.Adapt<VillaNumber>(), cancellationToken);

			if (createdVilla == null)
				return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to create villa." });

			if (await _unitOfWork.CompleteAsync(cancellationToken) > 0)
				_logger.LogInformation("Villa created successfully. ID: {villaNo}, Time: {Time}", createdVilla.VillaNo, DateTime.UtcNow);


			_response = new(statusCode: HttpStatusCode.Created, result: createdVilla);
			return CreatedAtAction(nameof(Get), new { villaNo = createdVilla.VillaNo }, _response);

		}
		catch (Exception ex)
		{
			_response = new(statusCode: HttpStatusCode.InternalServerError, isSuccess: false, errorMessages: new List<string>() { ex.ToString() });
			return _response;
		}
	}

	[HttpDelete("{villaNo:int}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[Authorize(Roles = "admin")]
	public async Task<ActionResult<ApiResponse>> DeleteVilla(int villaNo, CancellationToken cancellationToken = default)
	{
		try
		{
			if (!await _villaNumberRepository.IsExistsAsync(v => v.VillaNo == villaNo))
			{
				ModelState.AddModelError("ErrorMessages", "Invalid Vill Number Id");
				return BadRequest(ModelState);
			}

			var exists = await _unitOfWork.VillaNumber.IsExistsAsync(v => v.VillaNo == villaNo, cancellationToken);
			if (!exists)
				return NotFound(new { message = $"Villa with ID {villaNo} not found." });

			await _unitOfWork.VillaNumber.DeleteAsync(villaNo, cancellationToken);

			if (await _unitOfWork.CompleteAsync(cancellationToken) > 0)
				_logger.LogInformation("Villa deleted successfully. ID: {villaNo}, Time: {Time}", villaNo, DateTime.UtcNow);


			_response = new(statusCode: HttpStatusCode.NoContent);
			return Ok(_response);
		}
		catch (Exception ex)
		{
			_response = new(statusCode: HttpStatusCode.InternalServerError, isSuccess: false, errorMessages: new List<string>() { ex.ToString() });
			return _response;
		}
	}


	[HttpPut("{villaNo:int}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[Authorize(Roles = "admin")]
	public async Task<ActionResult<ApiResponse>> UpdateVilla(int villaNo, [FromBody] VillaNumberUpdateDTO villaRequest, CancellationToken cancellationToken = default)
	{
		try
		{
			if (villaNo != villaRequest.VillaNo)
				return BadRequest(new { message = "ID mismatch between route and payload." });


			if (!await _villaRepository.IsExistsAsync(v => v.Id == villaRequest.VillaId))
			{
				ModelState.AddModelError("ErrorMessages", "Invalid Vill Id");
				return BadRequest(ModelState);
			}

			if (!await _villaNumberRepository.IsExistsAsync(v => v.VillaNo == villaNo))
			{
				ModelState.AddModelError("ErrorMessages", "Invalid Vill Nubmer Id");
				return BadRequest(ModelState);
			}


			var existing = await _unitOfWork.VillaNumber.GetAsync(v => v.VillaNo == villaNo, cancellationToken: cancellationToken);
			if (existing == null)
				return NotFound(new { message = $"Villa with ID {villaNo} not found." });

			villaRequest.Adapt(existing);

			await _unitOfWork.VillaNumber.Update(villaNo, existing, cancellationToken);

			if (await _unitOfWork.CompleteAsync(cancellationToken) <= 0)
				return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Update failed. Please try again later." });

			_response = new(statusCode: HttpStatusCode.NoContent);
			return Ok(_response);
		}
		catch (Exception ex)
		{
			_response = new(statusCode: HttpStatusCode.InternalServerError, isSuccess: false, errorMessages: new List<string>() { ex.ToString() });
			return _response;
		}
	}

	[HttpPatch("{villaNo:int}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[Authorize(Roles = "admin")]
	public async Task<ActionResult<ApiResponse>> UpdatePartialVilla(int villaNo, [FromBody] JsonPatchDocument<VillaNumberUpdateDTO> patchDTO, CancellationToken cancellationToken = default)
	{
		try
		{
			if (patchDTO is null || villaNo <= 0)
				return BadRequest(new { message = "Invalid patch document or ID." });


			var villa = await _unitOfWork.VillaNumber.GetAsync(v => v.VillaNo == villaNo, tracking: false, cancellationToken: cancellationToken);
			if (villa == null)
				return NotFound(new { message = $"Villa with ID {villaNo} not found." });

			var villaToPatch = villa.Adapt<VillaNumberUpdateDTO>();
			patchDTO.ApplyTo(villaToPatch, ModelState);

			if (!ModelState.IsValid || !TryValidateModel(villaToPatch))
				return ValidationProblem(ModelState);

			villaToPatch.Adapt(villa); // map back to the tracked entity

			await _unitOfWork.VillaNumber.Update(villaNo, villa, cancellationToken);
			if (await _unitOfWork.CompleteAsync(cancellationToken) <= 0)
				return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Partial update failed." });

			_logger.LogInformation("Villa patched successfully. ID: {villaNo}, Time: {Time}", villaNo, DateTime.UtcNow);


			_response = new(statusCode: HttpStatusCode.NoContent);
			return Ok(_response);
		}
		catch (Exception ex)
		{
			_response = new(statusCode: HttpStatusCode.InternalServerError, isSuccess: false, errorMessages: new List<string>() { ex.ToString() });
			return _response;
		}
	}
}