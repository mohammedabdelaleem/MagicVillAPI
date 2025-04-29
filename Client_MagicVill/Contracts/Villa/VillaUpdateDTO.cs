using System.ComponentModel.DataAnnotations;

namespace Client_MagicVill.Contracts.Villa;

public record VillaUpdateDTO
{
	[Required]
	public int Id { get; set; }
	public string Name { get; set; }
	public string Details { get; set; }
	public double Rate { get; set; }
	public int SqFt { get; set; }
	public int Occupancy { get; set; }
	public string ImgUrl { get; set; }
	public string Amenity { get; set; }
}