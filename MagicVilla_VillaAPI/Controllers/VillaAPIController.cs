using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;


namespace MagicVilla_VillaAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class VillaAPIController : ControllerBase
	{
		private ApiResponse _response;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<VillaAPIController> _logger;

		public VillaAPIController(IUnitOfWork unitOfWork, ILogger<VillaAPIController> logger)
		{
			_unitOfWork = unitOfWork;
			_logger = logger;
		}

		[HttpGet("all")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<ApiResponse>> GetAll(CancellationToken cancellation = default)
		{
			try
			{
				var villas = await _unitOfWork.Villa.GetAllAsync(cancellationToken: cancellation);

				_response = new(statusCode: HttpStatusCode.OK, result: villas);

				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response = new(statusCode: HttpStatusCode.InternalServerError, isSuccess: false, errorMessages: new List<string>() { ex.ToString() });
				return _response;
			}
		}


		[HttpGet("{id:int}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<ApiResponse>> Get(int id, CancellationToken cancellationToken = default)
		{
			try
			{
				if (id <= 0)
					return BadRequest(new { message = "Invalid ID. ID must be greater than zero." });

				var villa = await _unitOfWork.Villa.GetAsync(v => v.Id == id, cancellationToken: cancellationToken);

				if (villa == null)
					return NotFound(new { message = $"Villa with ID {id} not found." });


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
		public async Task<ActionResult<ApiResponse>> Add([FromBody] VillaCreateDTO villaRequest, CancellationToken cancellationToken = default)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				var createdVilla = await _unitOfWork.Villa.AddAsync(villaRequest.Adapt<Villa>(), cancellationToken);

				if (createdVilla == null)
					return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to create villa." });

				if (await _unitOfWork.CompleteAsync(cancellationToken) > 0)
					_logger.LogInformation("Villa created successfully. ID: {Id}, Time: {Time}", createdVilla.Id, DateTime.UtcNow);


				_response = new(statusCode: HttpStatusCode.Created, result: createdVilla);
				return CreatedAtAction(nameof(Get), new { id = createdVilla.Id }, _response);

			}
			catch (Exception ex)
			{
				_response = new(statusCode: HttpStatusCode.InternalServerError, isSuccess: false, errorMessages: new List<string>() { ex.ToString() });
				return _response;
			}
		}

		[HttpDelete("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<ApiResponse>> DeleteVilla(int id, CancellationToken cancellationToken = default)
		{
			try
			{
				if (id <= 0)
					return BadRequest(new { message = "Invalid ID. ID must be greater than zero." });

				var exists = await _unitOfWork.Villa.IsExistsAsync(v => v.Id == id, cancellationToken);
				if (!exists)
					return NotFound(new { message = $"Villa with ID {id} not found." });

				await _unitOfWork.Villa.DeleteAsync(id, cancellationToken);

				if (await _unitOfWork.CompleteAsync(cancellationToken) > 0)
					_logger.LogInformation("Villa deleted successfully. ID: {Id}, Time: {Time}", id, DateTime.UtcNow);


				_response = new(statusCode: HttpStatusCode.NoContent);
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response = new(statusCode: HttpStatusCode.InternalServerError, isSuccess: false, errorMessages: new List<string>() { ex.ToString() });
				return _response;
			}
		}

		[HttpPut("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<ApiResponse>> UpdateVilla(/*[FromQuery]*/int id, [FromBody] VillaUpdateDTO villaRequest, CancellationToken cancellationToken = default)
		{
			try
			{
				if (id != villaRequest.Id)
					return BadRequest(new { message = "ID mismatch between route and payload." });

				var existing = await _unitOfWork.Villa.GetAsync(v => v.Id == id, cancellationToken: cancellationToken);
				if (existing == null)
					return NotFound(new { message = $"Villa with ID {id} not found." });

				villaRequest.Adapt(existing);

				await _unitOfWork.Villa.Update(id, existing, cancellationToken);

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

		[HttpPatch("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<ApiResponse>> UpdatePartialVilla(int id, [FromBody] JsonPatchDocument<VillaUpdateDTO> patchDTO, CancellationToken cancellationToken = default)
		{
			try
			{
				if (patchDTO is null || id <= 0)
					return BadRequest(new { message = "Invalid patch document or ID." });

				var villa = await _unitOfWork.Villa.GetAsync(v => v.Id == id, tracking: false, cancellationToken: cancellationToken);
				if (villa == null)
					return NotFound(new { message = $"Villa with ID {id} not found." });

				var villaToPatch = villa.Adapt<VillaUpdateDTO>();
				patchDTO.ApplyTo(villaToPatch, ModelState);

				if (!ModelState.IsValid || !TryValidateModel(villaToPatch))
					return ValidationProblem(ModelState);

				villaToPatch.Adapt(villa); // map back to the tracked entity

				await _unitOfWork.Villa.Update(id, villa, cancellationToken);
				if (await _unitOfWork.CompleteAsync(cancellationToken) <= 0)
					return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Partial update failed." });

				_logger.LogInformation("Villa patched successfully. ID: {Id}, Time: {Time}", id, DateTime.UtcNow);



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
}
