	using Microsoft.AspNetCore.Mvc;

	namespace MagicVilla_VillaAPI.Controllers;
	[Route("auth/v{version:apiVersion}/[controller]")]
	[ApiController]
	[ApiVersionNeutral]
	public class UsersController : ControllerBase
	{
		private readonly IUserRepository _userRepository;
		protected ApiResponse _response;
		public UsersController(IUserRepository userRepository)
		{
			_userRepository = userRepository;
			_response = new();
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequestDTO request, CancellationToken cancellationToken=default)
		{
			TokenDTO tokenDto = await _userRepository.LoginAsync(request, cancellationToken);

			if (tokenDto == null || string.IsNullOrEmpty(tokenDto.AccessToken))
			{
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("Invalid Username Or Password");
				return BadRequest(_response);
			}

			_response.StatusCode = HttpStatusCode.OK;
			_response.IsSuccess = true;
			_response.Result = tokenDto;
			return Ok(_response);

		}


		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterationRequestDTO request, CancellationToken cancellationToken = default)
		{
			bool isUinque = await _userRepository.IsUniqueUserAsync(request.UserName, cancellationToken);
			if (!isUinque)
			{
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("Username Actually Found, Choose Another One");
				return BadRequest(_response);
			}
			
			var user = await _userRepository.RegisterAsync(request, cancellationToken);
			if (user == null)
			{
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("Error While Registering");
				return BadRequest(_response);
			}

			_response.StatusCode = HttpStatusCode.OK;
			_response.IsSuccess = true;

			return Ok(_response);
		}


	[HttpPost("refresh")]
	public async Task<IActionResult> GetNewTokenFromRefreshToken([FromBody] TokenDTO tokenDTO, CancellationToken cancellationToken = default)
	{
		if (ModelState.IsValid)
		{
			//TODO: we can add if the refresh token is not expired : then return the same refresh token
			var tokenDtoResponse = await _userRepository.RefreshAccessToken(tokenDTO, cancellationToken);

			if (tokenDtoResponse == null || string.IsNullOrEmpty(tokenDtoResponse.AccessToken))
			{
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("Token Invalid");

			    return BadRequest(_response);
			}

			_response.StatusCode = HttpStatusCode.OK;
			_response.IsSuccess = true;
			_response.Result = tokenDtoResponse;
			return Ok(_response);

		}
		else
		{
			_response.StatusCode = HttpStatusCode.BadRequest;
			_response.IsSuccess = false;
			_response.ErrorMessages.Add("Token Invalid");
			return BadRequest(_response);
		}

	}


	[HttpPost("revoke")]
	public async Task<IActionResult> RevokeRefreshToken([FromBody] TokenDTO tokenDTO, CancellationToken cancellationToken = default)
	{
	
		if (ModelState.IsValid)
		{
			await _userRepository.RevokeRefreshTokenAsync(tokenDTO, cancellationToken);
			_response.StatusCode = HttpStatusCode.OK;
			_response.IsSuccess = true;
			return Ok(_response);
		}

		
			_response.StatusCode = HttpStatusCode.BadRequest;
			_response.IsSuccess = false;
			_response.Result ="Invalid Input";
			return BadRequest(_response);
	
	}

	// Exception Handling 

	[HttpGet("Error")]
	public async Task<IActionResult> Error()
	{
		throw new FileNotFoundException();
	}

}
