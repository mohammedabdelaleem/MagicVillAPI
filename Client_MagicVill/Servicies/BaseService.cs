using MagicVilla_Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace Client_MagicVill.Servicies;

//This class is responsible for sending HTTP requests (GET, POST, PUT, DELETE) to an API.
public class BaseService : IBaseService
{
	private readonly ITokenProvider _tokenProvider;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IApiMessageRequestBuilder _apiMessageRequestBuilder;
	protected readonly string _villaApiUrl;
	public ApiResponse _responseModel { get; set; }


	// in order to [ call the api ] we will using IHttpClientFactory that is already injected at DI
	public IHttpClientFactory _httpClient { get; set; }
	public BaseService(IHttpClientFactory httpClient, ILogger<BaseService> logger, ITokenProvider tokenProvider
		, IConfiguration configuration, IHttpContextAccessor httpContextAccessor,
		IApiMessageRequestBuilder apiMessageRequestBuilder)
	{
		_responseModel = new();
		_httpClient = httpClient;
		_villaApiUrl = configuration.GetValue<string>("UrlServices:VillaApi");
		_tokenProvider = tokenProvider;
		_httpContextAccessor = httpContextAccessor;
		_apiMessageRequestBuilder = apiMessageRequestBuilder;
	}


	// sends HTTP requests Dynamically.
	public async Task<T> SendAsync<T>(ApiRequest apiRequest, bool withBearer = true, CancellationToken cancellationToken= default)
	{
		try
		{

			var client = _httpClient.CreateClient("MagicAPI");
			// (In Program.cs, you must have configured this client with base URL, headers, etc.)


			// forbidden to use the same message object more than once 
			var messageFactory = () =>
			{
				return _apiMessageRequestBuilder.Build(apiRequest);
			};

			HttpResponseMessage httpResponseMessage = null;


			//if (!string.IsNullOrEmpty(apiRequest.Token))
			//{
			//	client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiRequest.Token);
			//}


			// send the HTTP request to the server and wait for the response
			httpResponseMessage = await SendWithRefreshTokenAsync(client, messageFactory, withBearer);


			ApiResponse finalApiResponse = new()
			{
				IsSuccess = false,
			};

			// Read and deserialize response

			// Read the API response body.
			// Deserialize it from JSON into the type T you expect(generic).
			// Return it to the caller.

			///var apiContent = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);


			// Problem is We Need To Stop If There are ErrorMessages , Is Success is false and Display The Error Messages
			// At My App , I Sure That My Response Is APIResponse .So We Do That.
			// If We Are At Any Other App Which Work With Generic We Need To Do This In Another Way.

			try
			{

				switch (httpResponseMessage.StatusCode)
				{
					case HttpStatusCode.NotFound:
						finalApiResponse.ErrorMessages.Add("Not Found");
						break;
					case HttpStatusCode.Forbidden:
						finalApiResponse.ErrorMessages.Add("Access Denied");
						break;
					case HttpStatusCode.Unauthorized:
						finalApiResponse.ErrorMessages.Add("Unauthorized");
						break;
					case HttpStatusCode.InternalServerError:
						finalApiResponse.ErrorMessages.Add("Internal Server Error");
						break;

					default:
						var apiContent = await httpResponseMessage.Content.	ReadAsStringAsync(cancellationToken);
						finalApiResponse.IsSuccess = true;
						finalApiResponse = JsonConvert.DeserializeObject<ApiResponse>(apiContent);
						break;
				}
			}
			catch (Exception ex)
			{
				finalApiResponse.ErrorMessages.Add($"Error Encountered : {ex.Message.ToString()}");

			}

			var result = JsonConvert.SerializeObject(finalApiResponse);
			var returnObj = JsonConvert.DeserializeObject<T>(result);
			return returnObj;

		}
		catch (AuthException) // now base service is configured to throw AuthException , We Need To Have An Extension Method that will capture this exception and redirect to Login Page==> and this will done by using exception filter
		{
			throw; 
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

	private async Task<HttpResponseMessage> SendWithRefreshTokenAsync(HttpClient httpClient,
		Func<HttpRequestMessage> httpRequestMessageFactory, bool withBearer = true)
	{
		if (!withBearer)
		{
			return await httpClient.SendAsync(httpRequestMessageFactory());
		}
		else // what if some of our end points don't require a token
		{

			TokenDTO tokenDTO = _tokenProvider.GetToken();
			if (tokenDTO != null || !string.IsNullOrEmpty(tokenDTO.AccessToken))
			{
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenDTO.AccessToken);
			}

			try
			{
				var response = await httpClient.SendAsync(httpRequestMessageFactory());

				if (response.IsSuccessStatusCode) // [200 - 299]
					return response;


				// if this fails we can send refresh token
				if (!response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.Unauthorized)
				{
					// generate new token from refresh token
					// signin with that token and then retry 

					await InvokeRefreshTokenEndpoint(httpClient, tokenDTO.AccessToken, tokenDTO.RefreshToken);
					response = await httpClient.SendAsync(httpRequestMessageFactory());
					return response;
				}

				return response;
			}
			catch (AuthException)
			{
				throw; // throw exception directly to the parent
			}
			catch (HttpRequestException httpRequestException)
			{
				if (httpRequestException.StatusCode == HttpStatusCode.Unauthorized)
				{
					// refresh token and retry the request
					await InvokeRefreshTokenEndpoint(httpClient, tokenDTO.AccessToken, tokenDTO.RefreshToken);
					return await httpClient.SendAsync(httpRequestMessageFactory());
				}

				throw;
			}


		}
	}


	private async Task InvokeRefreshTokenEndpoint(HttpClient httpClient, string existingAccessToken, string existingRefreshToken)
	{
		// this time we need to invoke the endpoint manually
		HttpRequestMessage message = new();
		message.Headers.Add("Accept", "application/json");

		message.RequestUri = new Uri($"{_villaApiUrl}/auth/{SD.CurrentApiVersion}/Users/refresh");
		message.Method = HttpMethod.Post;

		message.Content = new StringContent(JsonConvert.SerializeObject(new TokenDTO()
		{
			AccessToken = existingAccessToken,
			RefreshToken = existingRefreshToken
		}), Encoding.UTF8, "application/json");

		var response = await httpClient.SendAsync(message);
		var content = await response.Content.ReadAsStringAsync();
		var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(content);

		if (apiResponse?.IsSuccess != true)
		{
			// tokene refreshment failed so we can't use refresh token any more (we have to remove that ) and in top of that we need to sign out the user
			
			// if unauthorized automatically redirect the user to login page 
			// at that point we can throw a custome exception we can catch that and process it and redirect it to the login page	
			await _httpContextAccessor.HttpContext.SignOutAsync();
			_tokenProvider.ClearToken();
			throw new AuthException();
		}
		else
		{
			// my question? Why serialize then deserialize??
			var tokenDataStr = JsonConvert.SerializeObject(apiResponse.Result);
			var tokenDto = JsonConvert.DeserializeObject<TokenDTO>(tokenDataStr);

			if (tokenDto != null && !string.IsNullOrEmpty(tokenDto.AccessToken))
			{
				await SignInWithNewTokens(tokenDto);

				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenDto.AccessToken);
			}
		}
	}

	private async Task SignInWithNewTokens(TokenDTO tokenDTO)
	{
		var handler = new JwtSecurityTokenHandler();
		var jwt = handler.ReadJwtToken(tokenDTO.AccessToken);



		var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
		identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value));
		identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(c => c.Type == "role").Value));
		identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(c => c.Type == "unique_name" || c.Type == "name")?.Value ?? "User"));

		var principal = new ClaimsPrincipal(identity);
		await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

		_tokenProvider.SetToken(tokenDTO);
	}
}

