using Application.Ports.In.Iam;
using Infrastructure.Adapters.In.Dtos.Iam;
using Infrastructure.Adapters.Out.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Adapters.In.Controllers.Iam;

[ApiController]
[Route("api/iam/roles")]
[Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
public class RolesIamController(IRoleIamPort roleIamPort) : ControllerBase
{
    private readonly IRoleIamPort _roleIamPort = roleIamPort;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var roles = await _roleIamPort.GetAllRolesAsync(ct);
        return Ok(roles.Select(r => new RoleResponse(r.Id, r.Name ?? string.Empty)));
    }

    [HttpGet("{roleId}/scopes")]
    public async Task<IActionResult> GetScopes(string roleId, CancellationToken ct)
    {
        var scopes = await _roleIamPort.GetRoleScopesAsync(roleId, ct);
        return Ok(scopes.Select(s => new ScopeResponse(s.Id, s.Name, s.Description)));
    }

    [HttpPost("{roleId}/scopes/{scopeId:guid}")]
    public async Task<IActionResult> AssignScope(string roleId, Guid scopeId, CancellationToken ct)
    {
        await _roleIamPort.AssignScopeToRoleAsync(roleId, scopeId, ct);
        return NoContent();
    }

    [HttpDelete("{roleId}/scopes/{scopeId:guid}")]
    public async Task<IActionResult> RemoveScope(string roleId, Guid scopeId, CancellationToken ct)
    {
        await _roleIamPort.RemoveScopeFromRoleAsync(roleId, scopeId, ct);
        return NoContent();
    }
}

