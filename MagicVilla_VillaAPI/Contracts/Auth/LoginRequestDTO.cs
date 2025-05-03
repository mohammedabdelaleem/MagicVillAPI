namespace MagicVilla_VillaAPI.Contracts.Auth;


// if the user login success => we need to send the user details and token
public class LoginRequestDTO
{
	public string UserName { get; set; }
	public string Password { get; set; }

}
