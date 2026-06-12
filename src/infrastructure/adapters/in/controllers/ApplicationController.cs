using System.Net;
using Application.Ports.In;
using Core.Filters;
using Infrastructure.Adapters.Out.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Insfrastructure.Adapters.In;

[ApiController]
[Route("api")]
public class ApplicationController(IHelloWorldPort helloWorldPort) : ControllerBase
{
    private readonly IHelloWorldPort _helloWorldPort = helloWorldPort;

  [HttpGet("health")]
        [AllowAnonymous]
    public IActionResult Health()
    {
        return Ok("Healthy");
    }

    [HttpGet("hello")]
        [Authorize(Roles = AppRoles.User + "," + AppRoles.Admin)]
    [GeneralResponse("Saludo generado correctamente", HttpStatusCode.Created)]
    public IActionResult SayHello([FromQuery] string name)
    {
        var greeting = _helloWorldPort.SayHello(name);
        return Ok(greeting);
    }

    [HttpGet("admin/health")]
    [Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
    public IActionResult AdminHealth()
    {
        return Ok(new { status = "Admin healthy" });
    }

}