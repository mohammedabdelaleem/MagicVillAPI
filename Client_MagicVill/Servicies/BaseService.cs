using Client_MagicVill.Models;
using Client_MagicVill.Servicies.IServicies;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Text;

namespace Client_MagicVill.Servicies;

//This class is responsible for sending HTTP requests (GET, POST, PUT, DELETE) to an API.
public class BaseService : IBaseService
{
	public ApiResponse _responseModel { get; set; }


	// in order to [ call the api ] we will using IHttpClientFactory that is already injected at DI
	public IHttpClientFactory _httpClient { get; set; }
	public BaseService(IHttpClientFactory httpClient)
	{
		_responseModel = new();
		_httpClient = httpClient;
	}


	// sends HTTP requests Dynamically.
	public async Task<T> SendAsync<T>(ApiRequest apiRequest)
	{
		try
		{

			var client = _httpClient.CreateClient("MagicAPI"); 
			// (In Program.cs, you must have configured this client with base URL, headers, etc.)



			//  Build a message
			var message = new HttpRequestMessage();
			message.Headers.Add("Accept", "application/json");
			message.RequestUri = new Uri(apiRequest.Url);

			if (apiRequest.Data != null) // Post ,Put
			{
				// If you are sending data(e.g., for POST, PUT),
				// serialize the object to JSON and add it to the request body.

				// You Work With API - [WebService] , Any Platform speaks to any Platform So We Need The Standerd Format Like json when sending | recieving Data

				message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data),
				Encoding.UTF8, "application/json");
			}

			// Set HTTP method
			switch (apiRequest.ApiType)
			{
				case ApiType.POST:
					message.Method = HttpMethod.Post;
					break;
				case ApiType.PUT:
					message.Method = HttpMethod.Put;
					break;
				case ApiType.DELETE:
					message.Method = HttpMethod.Delete;
					break;
				default:
					message.Method = HttpMethod.Get;
					break;
			}

			// send the HTTP request to the server and wait for the response
			HttpResponseMessage apiResponse = null;
			apiResponse = await client.SendAsync(message);


			// Read and deserialize response

			// Read the API response body.
			// Deserialize it from JSON into the type T you expect(generic).
			// Return it to the caller.
			var apiContent = await apiResponse.Content.ReadAsStringAsync();
			var ApiResponse = JsonConvert.DeserializeObject<T>(apiContent);
			return ApiResponse;
		}
		catch (Exception ex)
		{

			var dto = new ApiResponse
			{
				ErrorMessages = new List<string> { Convert.ToString(ex.Message) },
				IsSuccess = false,
			};

			var result = JsonConvert.SerializeObject(dto);
			var ApiResponse = JsonConvert.DeserializeObject<T>(result);
			return ApiResponse;
		}

	}
}
