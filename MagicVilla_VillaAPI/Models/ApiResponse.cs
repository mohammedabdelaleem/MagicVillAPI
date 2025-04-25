using System.Net;

namespace MagicVilla_VillaAPI.Models;

// Standerize Respons
public class ApiResponse
{
	public ApiResponse(HttpStatusCode statusCode,
		object result= null, bool isSuccess= true, List<string> errorMessages = null)
	{
		StatusCode = statusCode;
		IsSuccess = isSuccess;
		ErrorMessages = errorMessages;
		Result = result ?? new();
	}

	public HttpStatusCode StatusCode { get; set; }
	public bool IsSuccess { get; set; }
	public List<string> ErrorMessages { get; set; }
	public object Result { get; set; }
}
