using System.ComponentModel.DataAnnotations;

namespace Client_MagicVill.Contracts.Auth;


// if the user login success => we need to send the user details and token
public class LoginRequestDTO
{
	
	public string UserName { get; set; }

	[DataType(DataType.Password)]
	public string Password { get; set; }

}
