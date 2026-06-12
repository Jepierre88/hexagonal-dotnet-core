using Application.Ports.In.Iam;
using Infrastructure.Adapters.In.Dtos.Iam;
using Infrastructure.Adapters.Out.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Adapters.In.Controllers.Iam;

[ApiController]
[Route("api/iam/users")]
[Authorize(Policy = AuthorizationPolicies.RequireAdminRole)]
public class UsersIamController(IUserIamPort userIamPort) : ControllerBase
{
    private readonly IUserIamPort _userIamPort = userIamPort;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var users = await _userIamPort.GetAllUsersAsync(ct);
        return Ok(users.Select(u => new UserIamResponse(u.Id, u.Email ?? string.Empty)));
    }

    [HttpGet("{userId}/scopes")]
    public async Task<IActionResult> GetEffectiveScopes(string userId, CancellationToken ct)
    {
        var scopes = await _userIamPort.GetUserEffectiveScopesAsync(userId, ct);
        return Ok(scopes.Select(s => new ScopeResponse(s.Id, s.Name, s.Description)));
    }

    [HttpGet("{userId}/scopes/direct")]
    public async Task<IActionResult> GetDirectScopes(string userId, CancellationToken ct)
    {
        var scopes = await _userIamPort.GetUserDirectScopesAsync(userId, ct);
        return Ok(scopes.Select(s => new ScopeResponse(s.Id, s.Name, s.Description)));
    }

    [HttpPost("{userId}/scopes/{scopeId:guid}")]
    public async Task<IActionResult> AssignScope(string userId, Guid scopeId, CancellationToken ct)
    {
        await _userIamPort.AssignScopeToUserAsync(userId, scopeId, ct);
        return NoContent();
    }

    [HttpDelete("{userId}/scopes/{scopeId:guid}")]
    public async Task<IActionResult> RemoveScope(string userId, Guid scopeId, CancellationToken ct)
    {
        await _userIamPort.RemoveScopeFromUserAsync(userId, scopeId, ct);
        return NoContent();
    }
}

