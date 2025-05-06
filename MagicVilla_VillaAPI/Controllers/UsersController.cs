	using Microsoft.AspNetCore.Mvc;

	namespace MagicVilla_VillaAPI.Controllers;
	[Route("auth/v{version:apiVersion}/[controller]")]
	[ApiController]
	[ApiVersionNeutral]
	public class UsersController : ControllerBase
	{
		private readonly IUserRepository _userRepository;
		//private readonly IUnitOfWork _unitOfWork;
		protected ApiResponse _response;
		public UsersController(IUserRepository userRepository, IUnitOfWork unitOfWork)
		{
			_userRepository = userRepository;
			//_unitOfWork = unitOfWork;
			_response = new();
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequestDTO request, CancellationToken cancellationToken=default)
		{
			LoginResponsDTO loginResponse = await _userRepository.Login(request, cancellationToken);

			if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
			{
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("Invalid Username Or Password");
				return BadRequest(_response);
			}

			_response.StatusCode = HttpStatusCode.OK;
			_response.IsSuccess = true;
			_response.Result = loginResponse;
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
			
			var user = await _userRepository.Register(request, cancellationToken);
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


	}
