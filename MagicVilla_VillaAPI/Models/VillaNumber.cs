using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVilla_VillaAPI.Models;

public class VillaNumber
{
	[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int VillaNo { get; set; }

	public string Details { get; set; }

	public DateTime CreatedAt { get; set; } = DateTime.Now;
	public DateTime UpdatedAt { get; set; }


	[ForeignKey(nameof(Villa))]
	public int VillaId { get; set; }

	public Villa Villa { get; set; }
}
