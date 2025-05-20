using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MagicVilla_VillaAPI.Filters;

public class CustomeExceptionFilter : IActionFilter

{
	public void OnActionExecuted(ActionExecutedContext context)
	{
		if(context.Exception is FileNotFoundException fileNotFoundException)
		{
			context.Result = new ObjectResult("file not found but handled in filter")
			{
				StatusCode = 501
			};
			context.ExceptionHandled = true; // don't override by ErrorHandling
		}
	}

	public void OnActionExecuting(ActionExecutingContext context)
	{
	}
}
