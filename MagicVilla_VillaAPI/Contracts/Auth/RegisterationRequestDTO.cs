namespace MagicVilla_VillaAPI.Contracts.Auth;


// if the registeration is successful ==> then we will return back the response with 200 OK Status Code
public class RegisterationRequestDTO
{
	public string UserName { get; set; }
	public string Name { get; set; }
	public string Password { get; set; }
	public string Role { get; set; }
}
