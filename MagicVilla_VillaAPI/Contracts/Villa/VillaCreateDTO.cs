namespace MagicVilla_VillaAPI.Contracts.Villa;

public record VillaCreateDTO
{

	public string Name { get; set; }
	public string Details { get; set; }
	public double Rate { get; set; }
	public int SqFt { get; set; }
	public int Occupancy { get; set; }
	public string ImgUrl { get; set; }
	public string Amenity { get; set; }

}