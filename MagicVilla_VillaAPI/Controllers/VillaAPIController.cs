using Mapster;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class VillaAPIController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<VillaAPIController> _logger;

		public VillaAPIController(IUnitOfWork unitOfWork, ILogger<VillaAPIController> logger)
		{
			_unitOfWork = unitOfWork;
			_logger = logger;
		}

		[HttpGet("all")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<IEnumerable<VillaDTO>>> GetAll(CancellationToken cancellation = default)
		{
			var villas = await _unitOfWork.Villa.GetAllAsync(cancellationToken: cancellation);
			return Ok(villas.Adapt<IEnumerable<VillaDTO>>());
		}

		[HttpGet("{id:int}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<VillaDTO>> Get(int id, CancellationToken cancellationToken = default)
		{
			if (id <= 0)
				return BadRequest(new { message = "Invalid ID. ID must be greater than zero." });

			var villa = await _unitOfWork.Villa.GetAsync(v => v.Id == id, cancellationToken: cancellationToken);

			if (villa == null)
				return NotFound(new { message = $"Villa with ID {id} not found." });

			return Ok(villa.Adapt<VillaDTO>());
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult> Add([FromBody] VillaCreateDTO villaRequest, CancellationToken cancellationToken = default)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var villa = await _unitOfWork.Villa.AddAsync(villaRequest.Adapt<Villa>(), cancellationToken);

			if (villa == null)
				return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to create villa." });

			if (await _unitOfWork.CompleteAsync(cancellationToken) > 0)
				_logger.LogInformation("Villa created successfully. ID: {Id}, Time: {Time}", villa.Id, DateTime.UtcNow);

			return CreatedAtAction(nameof(Get), new { id = villa.Id }, villa.Adapt<VillaDTO>());
		}

		[HttpDelete("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> DeleteVilla(int id, CancellationToken cancellationToken = default)
		{
			if (id <= 0)
				return BadRequest(new { message = "Invalid ID. ID must be greater than zero." });

			var exists = await _unitOfWork.Villa.IsExistsAsync(v => v.Id == id, cancellationToken);
			if (!exists)
				return NotFound(new { message = $"Villa with ID {id} not found." });

			await _unitOfWork.Villa.DeleteAsync(id, cancellationToken);

			if (await _unitOfWork.CompleteAsync(cancellationToken) > 0)
				_logger.LogInformation("Villa deleted successfully. ID: {Id}, Time: {Time}", id, DateTime.UtcNow);

			return NoContent();
		}

		[HttpPut("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO villaRequest, CancellationToken cancellationToken = default)
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

			return NoContent();
		}

		[HttpPatch("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult> UpdatePartialVilla(int id, [FromBody] JsonPatchDocument<VillaUpdateDTO> patchDTO, CancellationToken cancellationToken = default)
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
			return NoContent();
		}
	}
}
