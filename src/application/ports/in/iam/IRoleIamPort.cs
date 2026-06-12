using Domain.Iam;
using Microsoft.AspNetCore.Identity;

namespace Application.Ports.In.Iam;

public interface IRoleIamPort
{
    Task<IReadOnlyList<IdentityRole>> GetAllRolesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Scope>> GetRoleScopesAsync(string roleId, CancellationToken ct = default);
    Task AssignScopeToRoleAsync(string roleId, Guid scopeId, CancellationToken ct = default);
    Task RemoveScopeFromRoleAsync(string roleId, Guid scopeId, CancellationToken ct = default);
}
