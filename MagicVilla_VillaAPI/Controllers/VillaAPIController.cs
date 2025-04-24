using Mapster;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly IVillaStoreService _villaStoreService;

        public VillaAPIController(IVillaStoreService villaStoreService)
        {
            _villaStoreService = villaStoreService;
        }


        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaCreateDTO>>> GetAll(CancellationToken cancellation = default)
        {
            var villas = await _villaStoreService.GetAllAsync(cancellation);
            var response = villas.Adapt<IEnumerable<VillaDTO>>();

            return Ok(villas);
        }


        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDTO>> Get([FromRoute]int id, CancellationToken cancellationToken = default)
        {
            if (id == 0)
            {
                return BadRequest(new { message = "Id Can't Be Zero" });
            }

            var villa = await _villaStoreService.GetAsync(id, cancellationToken: cancellationToken);

            if (villa == null)
            {
                return NotFound();
            }

            return Ok(villa.Adapt<VillaDTO>());
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Add([FromBody] VillaCreateDTO villaRequset, CancellationToken cancellationToken = default)
        {
          
            Villa villa = await _villaStoreService.AddAsync(villaRequset.Adapt<Villa>());

            if (villa == null)
                return StatusCode(StatusCodes.Status500InternalServerError);


            return CreatedAtAction(nameof(Get), new { id = villa.Id }, villa);
        }



        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteVilla(int id, CancellationToken cancellationToken = default)
        {
            if (id == 0)
            {
                return BadRequest(new { message = "Id Can't Be Zero" });
            }

            var villaExists = await _villaStoreService.IsExistsAsync(id, cancellationToken);

            if (!villaExists)
            {
                return NotFound();
            }

            await _villaStoreService.DeleteAsync(id, cancellationToken);
            return NoContent();

        }



        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO villaRequest, CancellationToken cancellationToken = default)
        {
            if (id != villaRequest.Id)
                return BadRequest(new { message = "Check Your Input Id And Model" });

            if (!await _villaStoreService.EditAsync(id, villaRequest.Adapt<Villa>(), cancellationToken))
                return NotFound(new { message = $"ERROR : Poll {id} Not Updated" });

            return NoContent();

        }



        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdatePartialVilla(int id, [FromBody] JsonPatchDocument<VillaUpdateDTO> patchDTO, CancellationToken cancellationToken = default)
        {
            if (patchDTO is null || id <= 0)
            {
                return BadRequest("Invalid patch document or ID.");
            }

            // Track The Model With Id 
            var villa = await _villaStoreService.GetAsync(id, cancellationToken:cancellationToken, tracking:false);

            // i don't track the entity from here

            if (villa == null)
            {
                return NotFound(new { message = $"Poll with {nameof(id)} {id} not found" });
            }

            var villaToPatch = villa.Adapt<VillaUpdateDTO>(); // Request With Request  ==> Same Models

            patchDTO.ApplyTo(villaToPatch, ModelState);

            if (!ModelState.IsValid || !TryValidateModel(villaToPatch))
            {
                return ValidationProblem(ModelState);
            }

			villaToPatch.Adapt(villa); // Map back patched data to entity


			// Track The Same Model With Id : This is a big problem for EF Core ==> That is Imposible
			await _villaStoreService.EditAsync(id, villa, cancellationToken);

            return NoContent();
        }

    }
}