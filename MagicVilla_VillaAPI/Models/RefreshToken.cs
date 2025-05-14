using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models;

public class RefreshToken
{
	[Key]
	public int Id { get; set; }

	public string UserId { get; set; } // for which refresh token has been generated 
	public string JwtTokenId { get; set; } // unique jwt token id given to the access token
	public string Refresh_Token { get; set; }
	public bool IsValid { get; set; }
	public DateTime ExpiresAt { get; set; }




}
