using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Contracts.VillaNumber;

public record VillaNumberCreateDTO
{
	[Required]
	public int VillaNo { get; set; }

	public string Details { get; set; }

	[Required]
	public int VillaId { get; set; }

}