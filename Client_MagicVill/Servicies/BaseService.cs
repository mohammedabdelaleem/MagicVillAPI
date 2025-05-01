using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Client_MagicVill.Servicies;

//This class is responsible for sending HTTP requests (GET, POST, PUT, DELETE) to an API.
public class BaseService : IBaseService
{
	private readonly ILogger<BaseService> _logger;
	public ApiResponse _responseModel { get; set; }


	// in order to [ call the api ] we will using IHttpClientFactory that is already injected at DI
	public IHttpClientFactory _httpClient { get; set; }
	public BaseService(IHttpClientFactory httpClient, ILogger<BaseService> logger)
	{
		_responseModel = new();
		_httpClient = httpClient;
		_logger = logger;
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
			HttpResponseMessage apiResponse = await client.SendAsync(message);

			// Check and log if the response is null
			if (apiResponse == null)
			{
				_logger.LogError($"\n\nApi response is null. No status code available., please recheck {apiRequest.Url} \n\n");
			}
			else if (!apiResponse.IsSuccessStatusCode)
			{
				_logger.LogWarning($"\n\nAPI call returned non-success status code: {apiResponse.StatusCode}, please recheck {apiRequest.Url} \n\n");
			}


			// Read and deserialize response

			// Read the API response body.
			// Deserialize it from JSON into the type T you expect(generic).
			// Return it to the caller.
			var apiContent = await apiResponse.Content.ReadAsStringAsync();


			// Problem is We Need To Stop If There are ErrorMessages , Is Success is false and Display The Error Messages
			// At My App , I Sure That My Response Is APIResponse .So We Do That.
			// If We Are At Any Other App Which Work With Generic We Need To Do This In Another Way.

			try
			{
				ApiResponse response = JsonConvert.DeserializeObject<ApiResponse>(apiContent);

				if(apiResponse.StatusCode == HttpStatusCode.BadRequest || 
					apiResponse.StatusCode == HttpStatusCode.NotFound)
				{
					response.StatusCode = HttpStatusCode.BadRequest;
					response.IsSuccess = false;
				}

				var result = JsonConvert.SerializeObject(response );
				var returnObj = JsonConvert.DeserializeObject<T>(result);
				return returnObj;
			}
			catch (Exception ex)
			{
				var response = JsonConvert.DeserializeObject<T>(apiContent);
				return response;

			}

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
