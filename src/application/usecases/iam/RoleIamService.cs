using Application.Ports.In.Iam;
using Application.Ports.Out.Iam;
using Domain.Iam;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.Iam;

public class RoleIamService(IRoleIamRepository roleIamRepository, IScopeRepository scopeRepository) : IRoleIamPort
{
    private readonly IRoleIamRepository _roleIamRepository = roleIamRepository;
    private readonly IScopeRepository _scopeRepository = scopeRepository;

    public Task<IReadOnlyList<IdentityRole>> GetAllRolesAsync(CancellationToken ct = default)
        => _roleIamRepository.GetAllAsync(ct);

    public async Task<IReadOnlyList<Scope>> GetRoleScopesAsync(string roleId, CancellationToken ct = default)
    {
        await RequireRoleExistsAsync(roleId, ct);
        return await _roleIamRepository.GetScopesAsync(roleId, ct);
    }

    public async Task AssignScopeToRoleAsync(string roleId, Guid scopeId, CancellationToken ct = default)
    {
        await RequireRoleExistsAsync(roleId, ct);
        await RequireScopeExistsAsync(scopeId, ct);
        await _roleIamRepository.AddRoleScopeAsync(roleId, scopeId, ct);
    }

    public async Task RemoveScopeFromRoleAsync(string roleId, Guid scopeId, CancellationToken ct = default)
    {
        await RequireRoleExistsAsync(roleId, ct);
        await _roleIamRepository.RemoveRoleScopeAsync(roleId, scopeId, ct);
    }

    private async Task RequireRoleExistsAsync(string roleId, CancellationToken ct)
    {
        var role = await _roleIamRepository.GetByIdAsync(roleId, ct);
        if (role is null)
            throw new KeyNotFoundException($"Rol '{roleId}' no encontrado.");
    }

    private async Task RequireScopeExistsAsync(Guid scopeId, CancellationToken ct)
    {
        var scope = await _scopeRepository.GetByIdAsync(scopeId, ct);
        if (scope is null)
            throw new KeyNotFoundException($"Scope '{scopeId}' no encontrado.");
    }
}
