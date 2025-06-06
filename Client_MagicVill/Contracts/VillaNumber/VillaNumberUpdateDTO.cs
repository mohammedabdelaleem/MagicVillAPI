﻿using System.ComponentModel.DataAnnotations;

namespace Client_MagicVill.Contracts.VillaNumber;

public record VillaNumberUpdateDTO
{
	[Required]
	public int VillaNo { get; set; }

	public string Details { get; set; }


	[Required]
	public int VillaId { get; set; }
}