namespace Client_MagicVill.Servicies.IServicies;

public interface ITokenProvider
{
	// set token when user logs in
	void SetToken(TokenDTO tokenDTO);

	// retrive token we are making api calls
	TokenDTO GetToken();

	// clear or reset token when user logs out
	void ClearToken();

}
