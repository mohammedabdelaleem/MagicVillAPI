namespace Client_MagicVill.Servicies.IServicies;

public interface IApiMessageRequestBuilder
{
	public HttpRequestMessage Build(ApiRequest apiRequest);
}
