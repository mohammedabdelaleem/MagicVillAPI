namespace Client_MagicVill.Contracts.Villa;

public record VillaDTO
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Details { get; set; }
	public double Rate { get; set; }
	public int SqFt { get; set; }
	public int Occupancy { get; set; }
	public string? ImgUrl { get; set; }
	public string? ImageLocalPath { get; set; }
	
	// In the villa DTO We Don't Need the IFormFile , villaDto Will not Be Used To Create A Villa
	public string Amenity { get; set; }

}
