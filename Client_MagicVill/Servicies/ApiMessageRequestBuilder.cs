
using Newtonsoft.Json;
using System.Text;

namespace Client_MagicVill.Servicies;

public class ApiMessageRequestBuilder : IApiMessageRequestBuilder
{
	public HttpRequestMessage Build(ApiRequest apiRequest)
	{
			//  Build a message
			var message = new HttpRequestMessage();

			if (apiRequest.ContentType == ContentType.MultipartFormData)
			{
				message.Headers.Add("Accept", "*/*");
			}
			else
			{
				message.Headers.Add("Accept", "application/json");
			}


			message.RequestUri = new Uri(apiRequest.Url);



			if (apiRequest.ContentType == ContentType.MultipartFormData)
			{
				var content = new MultipartFormDataContent();

				foreach (var prop in apiRequest.Data.GetType().GetProperties()) // load all the properties we have on the apirequest.data (object)
				{
					var value = prop.GetValue(apiRequest.Data);

					if (value is FormFile)
					{
						var file = (FormFile)value;

						if (file != null)
						{
							content.Add(new StreamContent(file.OpenReadStream()), prop.Name, file.FileName);
						}
					}
					else
					{
						content.Add(new StringContent(value == null ? "" : value.ToString()), prop.Name);
					}
				}
				message.Content = content;
			}
			else
			{
				if (apiRequest.Data != null) // Post ,Put
				{
					// If you are sending data(e.g., for POST, PUT),
					// serialize the object to JSON and add it to the request body.

					// You Work With API - [WebService] , Any Platform speaks to any Platform So We Need The Standerd Format Like json when sending | recieving Data

					message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data),
					Encoding.UTF8, "application/json");
				}
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

			return message;
	}
}
