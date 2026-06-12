using Domain.Iam;
using Microsoft.AspNetCore.Identity;

namespace Application.Ports.Out.Iam;

public interface IRoleIamRepository
{
    Task<IReadOnlyList<IdentityRole>> GetAllAsync(CancellationToken ct = default);
    Task<IdentityRole?> GetByIdAsync(string roleId, CancellationToken ct = default);

    Task<IReadOnlyList<Scope>> GetScopesAsync(string roleId, CancellationToken ct = default);
    Task AddRoleScopeAsync(string roleId, Guid scopeId, CancellationToken ct = default);
    Task RemoveRoleScopeAsync(string roleId, Guid scopeId, CancellationToken ct = default);
}
