using System.ComponentModel.DataAnnotations;

namespace Client_MagicVill.Contracts.Villa;

public record VillaCreateDTO
{

	[Required, StringLength(maximumLength: 50,MinimumLength = 10, ErrorMessage ="Length Must Between (10,50)")]	
	public string Name { get; set; }
	public string Details { get; set; }

	[Required]
	public double Rate { get; set; }


	[Display(Name = "Squre Feets")]
	public int SqFt { get; set; }
	public int Occupancy { get; set; }


	[Display(Name = "Image Url")]
	public string ImgUrl { get; set; }

	public string Amenity { get; set; }

}