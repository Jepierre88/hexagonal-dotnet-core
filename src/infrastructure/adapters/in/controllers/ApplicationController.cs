using System.Net;
using Application.Ports.In;
using Core.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Insfrastructure.Adapters.In;

[ApiController]
[Route("api")]
public class ApplicationController(IHelloWorldPort helloWorldPort) : ControllerBase
{
    private readonly IHelloWorldPort _helloWorldPort = helloWorldPort;

  [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok("Healthy");
    }

    [HttpGet("hello")]
    [GeneralResponse("Saludo generado correctamente", HttpStatusCode.Created)]
    public IActionResult SayHello([FromQuery] string name)
    {
        var greeting = _helloWorldPort.SayHello(name);
        return Ok(greeting);
    }

}