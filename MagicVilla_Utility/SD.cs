namespace MagicVilla_Utility;
public static class SD
{

	public enum ApiType
	{
		GET,
		POST,
		PUT,
		DELETE
	}

	public readonly static string AccessToken = "JwtToken";
	public readonly static string RefreshToken = "RefreshToken";
	public readonly static int AccessTokenExpiresInNMinutes = 1;
	public readonly static int RefreshTokenExpiresInNDays = 2;



	public readonly static string CurrentApiVersion = "v2";
	public readonly static string Admin = "admin";
	public readonly static string Customer = "customer";

	public enum ContentType
	{
		Json ,
		MultipartFormData
	}
}
