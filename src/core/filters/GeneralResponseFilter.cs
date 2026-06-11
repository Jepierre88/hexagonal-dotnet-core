using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared.Interfaces;

namespace Core.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class GeneralResponseAttribute : ActionFilterAttribute
{
  private readonly string _successMessage;
  private readonly HttpStatusCode? _successStatusCode;

  public GeneralResponseAttribute(
    string successMessage = "Request processed successfully.",
    HttpStatusCode successStatusCode = HttpStatusCode.OK)
  {
    _successMessage = successMessage;
    _successStatusCode = successStatusCode == 0 ? null : successStatusCode;
  }

  public override void OnActionExecuted(ActionExecutedContext context)
  {
    if (context.Exception != null)
    {
      var response = new GeneralResponse<object>(
        "An error occurred while processing your request.",
        null,
        false,
        System.Net.HttpStatusCode.InternalServerError
      );
      context.Result = new ObjectResult(response)
      {
        StatusCode = (int)response.StatusCode
      };
      context.ExceptionHandled = true;
      return;
    }

    if (context.Result is ObjectResult objectResult)
    {
      var statusCode = _successStatusCode
        ?? (HttpStatusCode?)(objectResult.StatusCode ?? StatusCodes.Status200OK);
      var isSuccess = statusCode is >= HttpStatusCode.OK and < HttpStatusCode.Ambiguous;

      var response = new GeneralResponse<object>(
        _successMessage,
        objectResult.Value,
        isSuccess,
        statusCode.Value
      );
      context.Result = new ObjectResult(response)
      {
        StatusCode = (int)statusCode.Value
      };
    }
  }

}