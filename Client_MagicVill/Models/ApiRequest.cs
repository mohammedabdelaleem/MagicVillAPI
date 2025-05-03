
namespace Client_MagicVill.Models;

public class ApiRequest
{
	public ApiType ApiType { get; set; } // = ApiType.GET;
	public string Url { get; set; }
	public object Data { get; set; } // Post , Put 
	public string  Token { get; set; } // passing the token from front to back
}
