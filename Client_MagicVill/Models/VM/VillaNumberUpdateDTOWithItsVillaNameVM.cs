using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Client_MagicVill.Models.VM;

public class VillaNumberUpdateDTOWithItsVillaNameVM
{
	public VillaNumberUpdateDTO VillaNumber { get; set; } = new();


	[ValidateNever]
	public IEnumerable<SelectListItem> VillaList { get; set; }
}
