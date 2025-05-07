namespace MagicVilla_VillaAPI.Contracts.Auth;

public class LoginResponsDTO
{
	public UserDTO User { get; set; }
	public string Token { get; set; }
	//public string Role { get; set; } // role at token 
}
