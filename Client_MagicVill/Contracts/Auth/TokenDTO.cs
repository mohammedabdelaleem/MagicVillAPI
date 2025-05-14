namespace Client_MagicVill.Contracts.Auth;

public class TokenDTO
{
// All The User Data Needed Will be in the token	 
	public string AccessToken { get; set; }

	public string RefreshToken { get; set; }
}
