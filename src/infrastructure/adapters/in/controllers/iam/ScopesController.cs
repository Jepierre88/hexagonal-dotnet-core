using Application.Ports.In.Iam;
using Infrastructure.Adapters.In.Dtos.Iam;
using Infrastructure.Adapters.Out.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Adapters.In.Controllers.Iam;

[ApiController]
[Route("api/iam/scopes")]
[Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
public class ScopesController(IScopePort scopePort) : ControllerBase
{
    private readonly IScopePort _scopePort = scopePort;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var scopes = await _scopePort.GetAllScopesAsync(ct);
        return Ok(scopes.Select(s => new ScopeResponse(s.Id, s.Name, s.Description)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var scope = await _scopePort.GetScopeAsync(id, ct);
        return Ok(new ScopeResponse(scope.Id, scope.Name, scope.Description));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScopeRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var scope = await _scopePort.CreateScopeAsync(request.Name, request.Description, ct);
        return CreatedAtAction(nameof(GetById), new { id = scope.Id },
            new ScopeResponse(scope.Id, scope.Name, scope.Description));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateScopeRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var scope = await _scopePort.UpdateScopeAsync(id, request.Name, request.Description, ct);
        return Ok(new ScopeResponse(scope.Id, scope.Name, scope.Description));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _scopePort.DeleteScopeAsync(id, ct);
        return NoContent();
    }
}

