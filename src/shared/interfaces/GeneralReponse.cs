using System.Net;

namespace Shared.Interfaces;

public class GeneralResponse<T>
{
    public string Message { get; set; }
    public T? Data { get; set; }

  public bool Success { get; set; }
  public HttpStatusCode StatusCode { get; set; }

    public GeneralResponse(string message, T? data, bool success, HttpStatusCode statusCode)
    {
        Message = message;
        Data = data;
        Success = success;
        StatusCode = statusCode;
    }
}