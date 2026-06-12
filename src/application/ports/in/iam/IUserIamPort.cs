using Domain.Iam;

namespace Application.Ports.In.Iam;

public interface IUserIamPort
{
    Task<IReadOnlyList<AppUser>> GetAllUsersAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Scope>> GetUserEffectiveScopesAsync(string userId, CancellationToken ct = default);
    Task<IReadOnlyList<Scope>> GetUserDirectScopesAsync(string userId, CancellationToken ct = default);
    Task AssignScopeToUserAsync(string userId, Guid scopeId, CancellationToken ct = default);
    Task RemoveScopeFromUserAsync(string userId, Guid scopeId, CancellationToken ct = default);
}
