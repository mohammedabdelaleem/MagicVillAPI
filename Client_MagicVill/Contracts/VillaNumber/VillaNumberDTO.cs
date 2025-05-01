using System.ComponentModel.DataAnnotations;

namespace Client_MagicVill.Contracts.VillaNumber;

public record VillaNumberDTO
{
	[Required]
	public int VillaNo { get; set; }

	public string Details { get; set; }

	[Required]
	[Display(Name ="Villa")]
	public int VillaId { get; set; }

	public VillaDTO Villa { get; set; }
}
