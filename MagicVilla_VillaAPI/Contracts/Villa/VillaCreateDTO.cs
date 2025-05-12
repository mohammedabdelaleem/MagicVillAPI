namespace MagicVilla_VillaAPI.Contracts.Villa;

public record VillaCreateDTO
{

	public string Name { get; set; }
	public string Details { get; set; }
	public double Rate { get; set; }
	public int SqFt { get; set; }
	public int Occupancy { get; set; }

	public string? ImgUrl { get; set; }
	public IFormFile? Image { get; set; } // we need the file itself , and this will be at the IFormFile ,, and this is how the web project will passing the image down to the Api proect 
	public string Amenity { get; set; }

}