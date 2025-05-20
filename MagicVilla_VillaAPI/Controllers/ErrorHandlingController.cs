using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace MagicVilla_VillaAPI.Controllers;
// we have endpoint that throw an exception , but this exception doesn't handeled anywhere at the app
// we can do a custome implementation and tell our app , hey if the app throws any exception redirect that to this particular controller and that will handled all the exceptions.

[Route("ErrorHandling")]
[ApiController]
[AllowAnonymous] // no authorization 
[ApiVersionNeutral]
[ApiExplorerSettings(IgnoreApi = true)] // no need for swagger documentation
public class ErrorHandlingController : ControllerBase
{
	[Route("ProcessError")]
	public IActionResult ProcessError([FromServices] IHostEnvironment hostEnvironment)
	{

		if (hostEnvironment.IsDevelopment())
		{
			//custome login

			// get details about the exception
			//IExceptionHandlerFeature => Represents a feature containing the error of the original request to be examined by an
			//exception handler.
			var features = HttpContext.Features.Get<IExceptionHandlerFeature>();

			return Problem(
					detail:features.Error.StackTrace,
					title:features.Error.Message,
					instance:hostEnvironment.EnvironmentName
					);
		}
		else
		{
			return Problem();


	}
	}
}
