﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Client_MagicVill.Models.VM;

public class VillaNumberDeleteDTOWithItsVillaNameVM
{
	public VillaNumberDTO VillaNumber { get; set; } = new();

	[ValidateNever]
	public IEnumerable<SelectListItem> VillaList { get; set; }
}
