using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers;
// we have endpoint that throw an exception , but this exception doesn't handeled anywhere at the app
// we can do a custome implementation and tell our app , hey if the app throws any exception redirect that to this particular controller and that will handled all the exceptions.

// 
[Route("ErrorHandling")]
[ApiController]
[AllowAnonymous] // no authorization 
[ApiVersionNeutral]
[ApiExplorerSettings(IgnoreApi = true)] // no need for swagger documentation
public class ErrorHandlingController : ControllerBase
{
	[Route("ProcessError")]
	public IActionResult ProcessError() => Problem();	
}
