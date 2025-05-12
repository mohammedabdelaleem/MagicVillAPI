using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;


namespace MagicVilla_VillaAPI.Controllers.v1
{
	[Route("api/v{version:apiVersion}/[controller]")]
	[ApiController]
	[ApiVersion("1.0")] // in second version i need to enhance the functionality => user can upload villa image from web project and then save it using api
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



		/*
		 The [ResponseCache] attribute in ASP.NET Core 
		 is used to control how the response from an action is cached by clients and intermediate
		 proxies (like browsers or CDNs).
		 
		 Don't use with [HttpPost] – response caching only works with safe methods (e.g., GET).
		 */

		[HttpGet("all")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ResponseCache(Duration = 30)]
		public async Task<ActionResult<ApiResponse>> GetAll([FromQuery(Name = "filterOccupancy")] int? occupancy,
			[FromQuery] string? search
			, int pageSize = 0, int pageNumber = 1,
			CancellationToken cancellation = default)
		{
			try
			{
				IEnumerable<Villa> villas;

				// filter and search on your data
				if (occupancy > 0)
				{
					villas = await _unitOfWork.Villa.GetAllAsync(v => v.Occupancy == occupancy
					, pageSize: pageSize, pageNumber: pageNumber
						, cancellationToken: cancellation);
				}
				else
				{
					villas = await _unitOfWork.Villa.GetAllAsync(cancellationToken: cancellation, pageSize: pageSize, pageNumber: pageNumber
						);
				}

				if (!string.IsNullOrEmpty(search))
				{
					villas = villas.Where(v => v.Name.ToLower().Contains(search.ToLower()));
				}

				var pagination = new Pagination()
				{
					PageNumber = pageNumber,
					PageSize = pageSize,
				};

				Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));
				_response = new(statusCode: HttpStatusCode.OK, result: villas.Adapt<List<VillaDTO>>());

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
		[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)] // every time you need to go to DB ===> No Caching
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
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[Authorize(Roles = "admin")]
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
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[Authorize(Roles = "admin")]
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
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[Authorize(Roles = "admin")]
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

				await _unitOfWork.Villa.UpdateAsync(id, existing, cancellationToken);

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
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[Authorize(Roles = "admin")]

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

				await _unitOfWork.Villa.UpdateAsync(id, villa, cancellationToken);
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
