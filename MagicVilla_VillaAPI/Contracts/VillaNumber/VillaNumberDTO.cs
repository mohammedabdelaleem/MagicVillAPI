using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Contracts.VillaNumber;

public record VillaNumberDTO
{
	[Required]
	public int VillaNo { get; set; }

	public string Details { get; set; }

	[Required]
	public int VillaId { get; set; }

	public VillaDTO Villa { get; set; }
}
